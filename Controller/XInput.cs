using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class XInput
{
	const string DllName = "XInput9_1_0.dll";

	[DllImport(DllName)]
	public extern static uint XInputGetState(
		int index,  // [in] Index of the gamer associated with the device
		out XInputState state // [out] Receives the current state
	);

	[DllImport(DllName)]
	public extern static uint XInputSetState(
		int index,  // [in] Index of the gamer associated with the device
		ref XInputVibration vibration// [in, out] The vibration information to send to the controller
	);

	// -----------------------------------------------------------------------

	public enum XInputButtons : ushort
	{
		DPadUp = 0x1,
		DPadDown = 0x2,
		DPadLeft = 0x4,
		DPadRight = 0x8,
		Start = 0x10,
		Back = 0x20,
		LeftThumb = 0x40,
		RightThumb = 0x80,
		LeftShoulder = 0x100,
		RightShoulder = 0x200,
		A = 0x1000,
		B = 0x2000,
		X = 0x4000,
		Y = 0x8000
	};

	// -----------------------------------------------------------------------

	public struct XInputVibration
	{
		public ushort LeftMotorSpeed;
		public ushort RightMotorSpeed;

		public XInputVibration(ushort left, ushort right)
		{
			this.LeftMotorSpeed = left;
			this.RightMotorSpeed = right;
		}
	}

	// -----------------------------------------------------------------------

	public struct XInputState
	{
		public uint PacketNumber;
		public XInputGamepad Gamepad;

		public bool Equals(XInputState rhs)
		{
			return PacketNumber == rhs.PacketNumber
					&& Gamepad.Equals(rhs.Gamepad);
		}

		public override bool Equals(object obj)
		{
			if (obj is XInputState)
				return this.Equals((XInputState)obj);
			return false;
		}

		public override int GetHashCode()
		{
			return (int)PacketNumber;
		}

		public override string ToString()
		{
			return string.Format("({0} {1})", PacketNumber, Gamepad);
		}
	}

	// -----------------------------------------------------------------------

	public struct XInputGamepad
	{
		public XInputButtons Buttons;
		public byte LeftTrigger;
		public byte RightTrigger;
		public short LeftThumbX;
		public short LeftThumbY;
		public short RightThumbX;
		public short RightThumbY;

		public bool IsButtonDown(XInputButtons button)
		{
			return (Buttons & button) != 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is XInputGamepad)
				return this.Equals((XInputGamepad)obj);
			return false;
		}

		public override int GetHashCode()
		{
			return (int)Buttons + LeftTrigger + RightTrigger;
		}

		public bool Equals(XInputGamepad rhs)
		{
			return
				Buttons == rhs.Buttons
				&& LeftTrigger == rhs.LeftTrigger
				&& RightTrigger == rhs.RightTrigger
				&& LeftThumbX == rhs.LeftThumbX
				&& LeftThumbY == rhs.LeftThumbY
				&& RightThumbX == rhs.RightThumbX
				&& RightThumbY == rhs.RightThumbY;
		}

		public override string ToString()
		{
			return string.Format("({0} {1} {2} {3} {4} {5} {6})", Buttons, LeftTrigger, RightTrigger, LeftThumbX, LeftThumbY, RightThumbX, RightThumbY);
		}
	}

	public static int GetNumControllers()
	{
		int numControllers = 0;

		for (int i = 0; i < 4; i++)
		{
            if (IsControllerConnected(i))
				numControllers++;
		}

		return numControllers;
	}

	public static bool IsControllerConnected(int index)
	{
		XInputState state;
		uint ret = XInputGetState(index, out state);

		if (ret == 0)
			return true;

		return false;
	}
}