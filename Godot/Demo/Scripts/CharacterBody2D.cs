using Godot;
using System;

public partial class CharacterBody2D : Godot.CharacterBody2D
{
	private AnimatedSprite2D _sprite2d;
	public const float Speed = 200.0f;

	public override void _Ready()
	{
		_sprite2d = GetNode<AnimatedSprite2D>("animations");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}
		//GD.Print(direction.X);
		
		if(direction.X<0 || direction.Y!=0)
			_sprite2d.Animation = "Go_left";
		else if(direction.X>0)
			_sprite2d.Animation = "Go_right";
		else
			_sprite2d.Animation = "Still";
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
