#if TOOLS
using Godot;
using System;
using LayeredWFC;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

[Tool]
public partial class AddTilesWindow : Window
{
	private LoadTileSetDialog _fileDialog;
	private Button _spritesButton, _collisionButton, _cancelButton, _addTiles;
	private Label _spritesLabel;
	private OptionButton _sceneListButton, _tileSizeListButton;
	private string _apiUrl;
	private AcceptDialog _modelTrainingAlert;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fileDialog= GetNode("LoadTileSetDialog") as LoadTileSetDialog;
		_spritesButton = GetNode("SpritesButton") as Button;
		_spritesButton.Pressed += OpenSpritesDialog;
		_fileDialog.FileSelected += OnConfirmDialog;
		_spritesLabel = GetNode("FileSprites") as Label;
		_cancelButton = GetNode("Cancel") as Button;
		_cancelButton.Pressed += OnCancel;
		_sceneListButton = GetNode("SceneBox") as OptionButton;
		foreach(var scene in SearchScenes("res://")){
			_sceneListButton.AddItem(scene);
		}
		_tileSizeListButton = GetNode("TileSizeBox") as OptionButton;
		_addTiles = GetNode("Add tiles") as Button;
		_addTiles.Pressed += OnAddTiles;
		_modelTrainingAlert = GetNode("ModelUnderTraining") as AcceptDialog;
	}
	
	public void OpenSpritesDialog()
	{
		_fileDialog.ClearFilters();
		_fileDialog.DialogCaller = CallerEnum.Sprites;
		_fileDialog.AddFilter("*.png","Images");
		_fileDialog.Popup();
	}
	
	public void OnConfirmDialog(String path)
	{
		_spritesLabel.Text = path;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			this.Hide();
	}
	
	public void OnCancel(){
		this.Hide();
	}
	
	public void ShowPopUp(string apiUrl){
		_apiUrl = apiUrl;
		this.Show();
	}
	
	public async void OnAddTiles(){
		var sizeItemId = _tileSizeListButton.GetSelectedId();
		var itemText = _tileSizeListButton.GetItemText(sizeItemId);
		var sceneItemId = _sceneListButton.GetSelectedId();
		var sceneName = _sceneListButton.GetItemText(sceneItemId);
		var tileMapService = new TileMapService(_spritesLabel.Text,sceneName, Int32.Parse(itemText), _apiUrl);
		if(! (await tileMapService.IsFreeToProcess()))
			await tileMapService.AddTilesToMap();
		else
			_modelTrainingAlert.Show();
		this.Hide();
	}
	
	 private IEnumerable<string> SearchScenes(string directoryPath)
	{

		var directory = DirAccess.Open(directoryPath);
		if (directory is null)
		{
			GD.PrintErr("Failed to open directory: " + directoryPath);
			return new List<string>();
		}

		var scenes = new List<string>();
		foreach(var dir in directory.GetDirectories()){
			scenes.AddRange(SearchScenes(directoryPath+dir+"/"));
		}
		foreach(var file in directory.GetFiles().Distinct()){
			if(file.EndsWith(".tscn") || file.EndsWith(".scn"))
				scenes.Add(file.Replace(".tscn","").Replace(".scn",""));
		}
		return scenes;
	}
	
	private WFCConfig ReadConfigFile(string filePath)
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		var configuration = JsonSerializer.Deserialize<WFCConfig>(content);
		return configuration;
	}
}
#endif
