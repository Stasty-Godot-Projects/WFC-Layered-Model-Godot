@tool
extends Control

var create_model_window

func _ready():
	create_model_window = $CreateModelWindow



func _on_get_main_model_window_pressed():
	create_model_window.ShowPopUp()
