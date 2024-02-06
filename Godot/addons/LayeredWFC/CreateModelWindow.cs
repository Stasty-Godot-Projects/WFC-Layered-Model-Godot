#if TOOLS
using Godot;
using System;
using LayeredWFC.Plugin;

[Tool]
public partial class CreateModelWindow : Window
{
	private LoadTileSetDialog _fileDialog;
	private Button _spritesButton, _collisionButton, _tileRulesButton, _cancelButton, _createModel;
	private Label _spritesLabel, _collisionsLabel, _tilesLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fileDialog= GetNode("LoadTileSetDialog") as LoadTileSetDialog;
		_spritesButton = GetNode("SpritesButton") as Button;
		_spritesButton.Pressed += OpenSpritesDialog;
		_collisionButton = GetNode("CollisionsButton") as Button;
		_collisionButton.Pressed += OpenCollisionDialog;
		_tileRulesButton = GetNode("TilesRulesButton") as Button;
		_tileRulesButton.Pressed += OpenTilesRulesDialog;
		_fileDialog.FileSelected += OnConfirmDialog;
		_spritesLabel = GetNode("FileSprites") as Label;
		_collisionsLabel = GetNode("FileCollisions") as Label;
		_tilesLabel = GetNode("FileTileRules") as Label;
		_cancelButton = GetNode("Cancel") as Button;
		_cancelButton.Pressed += OnCancel;
	}
	
	public void OpenSpritesDialog(){
		_fileDialog.DialogCaller = CallerEnum.Sprites;
		_fileDialog.Popup();
	}
	
	public void OpenCollisionDialog(){
		_fileDialog.DialogCaller = CallerEnum.Collisions;
		_fileDialog.Popup();
	}
	
	public void OpenTilesRulesDialog(){
		_fileDialog.DialogCaller = CallerEnum.TileRules;
		_fileDialog.Popup();
	}
	
	public void OnConfirmDialog(String path)
	{
		switch(_fileDialog.DialogCaller)
		{
			case CallerEnum.TileRules:
				_tilesLabel.Text = path;
				break;
			case CallerEnum.Collisions:
				_collisionsLabel.Text = path;
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
	
	
	
}
#endif
