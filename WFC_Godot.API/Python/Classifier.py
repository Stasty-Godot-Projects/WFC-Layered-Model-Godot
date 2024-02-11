import skimage.transform as tr
import numpy as np
from sklearn.model_selection import train_test_split
from tensorflow import keras
from keras import backend as K
from PIL import Image
import matplotlib.pyplot as plt
from sklearn.preprocessing import OneHotEncoder
from tensorflow.keras.models import load_model


class ModelClassifier:

    def CreateModel(size, channelNumber, numberOfClasses):
        if K.image_data_format() == "channels_first":
            shape = (channelNumber, size, size)
        else:
            shape = (size, size, channelNumber)
        model = keras.Sequential([
            keras.layers.Conv2D(64, (3,3), activation='relu', input_shape=shape),
            keras.layers.MaxPooling2D(2,2),
            keras.layers.Conv2D(64, (3,3), activation='relu'),
            keras.layers.MaxPooling2D(2,2),
            keras.layers.Flatten(),
            keras.layers.Dropout(0.5),
            keras.layers.Dense(256, activation='relu'),
            keras.layers.Dense(512, activation='relu'),
            keras.layers.Dense(1024, activation='relu'),
            
            keras.layers.Dense(numberOfClasses, activation="softmax")
        ])
        model.summary()

        model.compile(optimizer='adam', loss="sparse_categorical_crossentropy", metrics=['accuracy'])
        return model

    def DataPreparation(image, labelsSides, labelsCorner, tileSize):
        labelsSides = np.asarray(labelsSides)
        labelsSides = np.asarray(labelsCorner)
        image = np.asarray(image)
        [labelsSides,sidesCategories] = preprocess_labels(labelsSides)
        [labelsCorner,sornerCategories] = preprocess_labels(labelsCorner)


        image = ImagesRotation(image,tileSize)

        np.savez_compressed("SidesCategories.npz", sidesCategories)
        np.savez_compressed("CornerCategories.npz", sornerCategories)

        return [image,labelsSides,labelsCorner]

    def ImagesRotation(image,tileSize):
        tiles = [image[x:x+tileSize, y:y+tileSize] for x in range(0, image.shape[0], tileSize) for y in range(0, image.shape[1], tileSize)]
        new_images = np.zeros((len(tiles)*4, tileSize, tileSize, image.shape[2]), dtype=image.dtype)
        for i, tile in enumerate(tiles):
            for j in range(4):
                new_images[i*4+j] = np.rot90(tile, j)
        return new_images

    def TrainModels(data):
        train_images, test_images, train_labelsSides, test_labelsSides, train_labelsCorner, test_labelsCorner = train_test_split(data[0], data[1], data[2], test_size=0.2)
        SidesModel = CreateModel(data[0].shape[1],data[0].shape[-1],data[1].shape[1])
        CornersModel = CreateModel(data[0].shape[1],data[0].shape[-1],data[2].shape[1])
        if K.image_data_format() == "channels_first":
            train_images = train_images.reshape(train_images.shape[0], data[0].shape[-1],data[0].shape[1],data[0].shape[1])
            test_images = test_images.reshape(test_images.shape[0], data[0].shape[-1],data[0].shape[1],data[0].shape[1])
        else:
            train_images = train_images.reshape(train_images.shape[0], data[0].shape[1],data[0].shape[1],data[0].shape[-1])
            test_images = test_images.reshape(test_images.shape[0], data[0].shape[1],data[0].shape[1],data[0].shape[-1])
        callback = keras.callbacks.EarlyStopping(monitor='loss',patience=3)
        trained_Sides = SidesModel.fit(train_images, train_labelsSides, epochs=3, validation_data=(test_images,test_labelsSides), callbacks=[callback])  
        trained_Corners = CornersModel.fit(train_images, train_labelsCorner, epochs=3, validation_data=(test_images,test_labelsCorner), callbacks=[callback]) 

        SidesModel.save("Sides.h5")
        CornersModel.save("Corners.h5")
        return "ready"

    def onehot_to_categories(onehot_labels, category_names):
        categories = [category_names[np.argmax(label)] for label in onehot_labels]
        return categories

    def LoadModelAndPeridct(images, tileSize):
        image = np.asarray(image)        
        sidesCategories = np.load("SidesCategories.npz")['arr_0']
        cornerCategories = np.load("SidesCategories.npz")['arr_0']

        sidesModel = load_model('Sides.h5')
        cornersModel = load_model('Corners.h5')

        image = ImagesRotation(image,tileSize)

        if K.image_data_format() == "channels_first":
            images = images.reshape(images.shape[0], tileSize,tileSize,image.shape[-1])
        else:
            images = images.reshape(images.shape[0], tileSize,tileSize,image.shape[-1])

        sidesPredicted = sidesModel.predict(images)
        cornersPredicted = cornersModel.predict(images)

        sidesDeencoded = np.zeros(sidesPredicted.shape[0])
        cornersDeencoded = np.zeros(sidesPredicted.shape[0])
        for i in sidesPredicted.shape[0]:
            sidesDeencoded[i] = onehot_to_categories(sidesPredicted[i],sidesCategories)
            cornersDeencoded[i] = onehot_to_categories(cornersPredicted[i],cornerCategories)
        sidesDeencoded =sidesDeencoded.reshape(sidesDeencoded.shape[0]/4,4)
        cornersDeencoded =cornersDeencoded.reshape(cornersDeencoded.shape[0]/4,4)
        return [sidesDeencoded,cornersDeencoded]


    def preprocess_labels(labels):
        labels_flat = labels.flatten().reshape(-1, 1)
        encoder = OneHotEncoder(sparse=False)
        labels_onehot = encoder.fit_transform(labels_flat)
        return [labels_onehot, encoder.categories_[0]]