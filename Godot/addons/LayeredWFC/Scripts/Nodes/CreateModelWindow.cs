#if TOOLS
using Godot;
using System;
using LayeredWFC;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

[Tool]
public partial class CreateModelWindow : Window
{
	private LoadTileSetDialog _fileDialog;
	private Button _spritesButton, _collisionButton, _tileRulesButton, _cancelButton, _createModel;
	private Label _spritesLabel, _tilesLabel;
	private OptionButton _sceneListButton, _tileSizeListButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fileDialog= GetNode("LoadTileSetDialog") as LoadTileSetDialog;
		_spritesButton = GetNode("SpritesButton") as Button;
		_spritesButton.Pressed += OpenSpritesDialog;
		_tileRulesButton = GetNode("TilesRulesButton") as Button;
		_tileRulesButton.Pressed += OpenTilesRulesDialog;
		_fileDialog.FileSelected += OnConfirmDialog;
		_spritesLabel = GetNode("FileSprites") as Label;
		_tilesLabel = GetNode("FileTileRules") as Label;
		_cancelButton = GetNode("Cancel") as Button;
		_cancelButton.Pressed += OnCancel;
		_sceneListButton = GetNode("SceneBox") as OptionButton;
		foreach(var scene in SearchScenes("res://")){
			_sceneListButton.AddItem(scene);
		}
		_tileSizeListButton = GetNode("TileSizeBox") as OptionButton;
		_createModel = GetNode("Create model") as Button;
		_createModel.Pressed += OnCreateModel;
	}
	
	public void OpenSpritesDialog()
	{
		_fileDialog.ClearFilters();
		_fileDialog.DialogCaller = CallerEnum.Sprites;
		_fileDialog.AddFilter("*.png","Images");
		_fileDialog.Popup();
	}
	
	public void OpenTilesRulesDialog()
	{
		_fileDialog.ClearFilters();
		_fileDialog.DialogCaller = CallerEnum.TileRules;
		_fileDialog.AddFilter("*.json","ConfigFile");
		_fileDialog.Popup();
	}
	
	public void OnConfirmDialog(String path)
	{
		switch(_fileDialog.DialogCaller)
		{
			case CallerEnum.TileRules:
				_tilesLabel.Text = path;
				break;
			case CallerEnum.Sprites:
			default:
				_spritesLabel.Text = path;
				break;
		}
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			this.Hide();
	}
	
	public void OnCancel(){
		this.Hide();
	}
	
	public void ShowPopUp(){
		this.Show();
	}
	
	public void OnCreateModel(){
		var sizeItemId = _tileSizeListButton.GetSelectedId();
		var itemText = _tileSizeListButton.GetItemText(sizeItemId);
		var tileMapService = new TileMapService(_spritesLabel.Text,"Demo", Int32.Parse(itemText));
		var config = ReadConfigFile(_tilesLabel.Text);
		tileMapService.TilesDescriptions = config.TilesDescription;
		tileMapService.SidesKind = config.SidesDescription;
		tileMapService.CreateTileMap();
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
