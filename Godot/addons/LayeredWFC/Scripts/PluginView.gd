@tool
extends Control

var create_model_window
var add_tiles_window
var api_url_edit

func _ready():
	create_model_window = $CreateModelWindow
	api_url_edit = $VBoxContainer/HBoxContainer/ApiURLEdit
	add_tiles_window = $AddTilesWindow

func _on_get_main_model_window_pressed():
	var text = api_url_edit.get_text()
	if !text:
		no_url_popup()
	else:
		create_model_window.ShowPopUp(text)



func no_url_popup():
	$AcceptDialog.title = "No URL"
	$AcceptDialog.dialog_text = "Provide URL of your API"
	$AcceptDialog.popup_centered()
	$AcceptDialog.show()


func _on_add_tiles_button_pressed():
	var text = api_url_edit.get_text()
	if !text:
		no_url_popup()
	else:
		add_tiles_window.ShowPopUp(text)
