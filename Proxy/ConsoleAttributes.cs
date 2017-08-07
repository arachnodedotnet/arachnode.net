#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

// <summary>The Org.Mentalis.Utilities.ConsoleAttributes namespace defines classes that can be used to interact with the console to change its layout and behavior.</summary>

namespace Arachnode.Proxy
{
    /// <summary>
    /// The CONSOLE_CURSOR_INFO structure contains information about the console cursor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CONSOLE_CURSOR_INFO
    {
        /// <summary>Specifies a number between 1 and 100, indicating the percentage of the character cell that is filled by the cursor. The cursor appearance varies, ranging from completely filling the cell to showing up as a horizontal line at the bottom of the cell.</summary>
        public int dwSize;

        /// <summary>Specifies the visibility of the cursor. If the cursor is visible, this member is TRUE (nonzero).</summary>
        public int bVisible;
    }

    /// <summary>
    /// The COORD structure defines the coordinates of a character cell in a console screen buffer. The origin of the coordinate system (0,0) is at the top, left cell of the buffer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD
    {
        /// <summary>Horizontal or column value.</summary>
        public short x;

        /// <summary>Vertical or row value.</summary>
        public short y;
    }

    /// <summary>
    /// The SMALL_RECT structure defines the coordinates of the upper left and lower right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SMALL_RECT
    {
        /// <summary>Specifies the x-coordinate of the upper left corner of the rectangle.</summary>
        public short Left;

        /// <summary>Specifies the y-coordinate of the upper left corner of the rectangle.</summary>
        public short Top;

        /// <summary>Specifies the x-coordinate of the lower right corner of the rectangle.</summary>
        public short Right;

        /// <summary>Specifies the y-coordinate of the lower right corner of the rectangle.</summary>
        public short Bottom;
    }

    /// <summary>
    /// The CONSOLE_SCREEN_BUFFER_INFO structure contains information about a console screen buffer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CONSOLE_SCREEN_BUFFER_INFO
    {
        /// <summary>Specifies the size, in character columns and rows, of the screen buffer.</summary>
        public COORD dwSize;

        /// <summary>Specifies the column and row coordinates of the cursor in the screen buffer.</summary>
        public COORD dwCursorPosition;

        /// <summary>Specifies the foreground (text) and background color attributes to be used for characters that are written to a screen buffer by the WriteFile and WriteConsole functions, or echoed to a screen buffer by the ReadFile and ReadConsole functions. The attribute values are some combination of the following values: FOREGROUND_BLUE, FOREGROUND_GREEN, FOREGROUND_RED, FOREGROUND_INTENSITY, BACKGROUND_BLUE, BACKGROUND_GREEN, BACKGROUND_RED, and BACKGROUND_INTENSITY.</summary>
        public short wAttributes;

        /// <summary>Specifies a SMALL_RECT structure that contains the screen buffer coordinates of the upper-left and lower-right corners of the display window.</summary>
        public SMALL_RECT srWindow;

        /// <summary>Specifies the maximum size of the console window, given the current screen buffer size and font and the screen size.</summary>
        public COORD dwMaximumWindowSize;
    }

    /// <summary>Enumerates all available colors for the forecolor or the backcolor of the console.</summary>
    public enum ConsoleColor
    {
        /// <summary>Black</summary>
        Black = 0,
        /// <summary>Red</summary>
        Red = 1,
        /// <summary>Light red</summary>
        LightRed = 2,
        /// <summary>Green</summary>
        Green = 3,
        /// <summary>Light green</summary>
        LightGreen = 4,
        /// <summary>Blue</summary>
        Blue = 5,
        /// <summary>Light blue</summary>
        LightBlue = 6,
        /// <summary>Gold</summary>
        Gold = 7,
        /// <summary>Yellow</summary>
        Yellow = 8,
        /// <summary>Cyan</summary>
        Cyan = 9,
        /// <summary>Light cyan</summary>
        LightCyan = 10,
        /// <summary>Purple</summary>
        Purple = 11,
        /// <summary>Light purple</summary>
        LightPurple = 12,
        /// <summary>Gray</summary>
        Gray = 13,
        /// <summary>White</summary>
        White = 14
    }

    /// <summary>The ConsoleAttributes class can change several attributes of your console window.</summary>
    /// <example>
    /// The following example wil change the forecolor of te console, disable 'EchoInput', ask for a string and show that string.
    ///	<code>
    ///	ConsoleAttributes.ForeColor = ConsoleColor.White;
    /// Console.Write("Please enter your password: ");
    /// ConsoleAttributes.EchoInput = false;
    /// string ThePass = Console.ReadLine();
    /// ConsoleAttributes.EchoInput = true;
    /// ConsoleAttributes.ForeColor = ConsoleColor.Gray;
    /// Console.WriteLine("");
    /// Console.WriteLine("The password you entered was: " + ThePass);
    /// Console.WriteLine("Press enter to exit...");
    /// Console.Read();
    /// </code>
    /// </example>
    public class ConsoleAttributes
    {
        /// <summary>
        /// Lists all the possible background color values.
        /// </summary>
        private static readonly int[] BacgroundColors = {0x0, 0x40, 0x80 | 0x40, 0x20, 0x80 | 0x20, 0x10, 0x80 | 0x10, 0x40 | 0x20, 0x80 | 0x40 | 0x20, 0x20 | 0x10, 0x80 | 0x20 | 0x10, 0x40 | 0x10, 0x80 | 0x40 | 0x10, 0x40 | 0x20 | 0x10, 0x80 | 0x40 | 0x20 | 0x10};

        /// <summary>
        /// Lists all the possible foreground color values.
        /// </summary>
        private static readonly int[] ForegroundColors = {0x0, 0x4, 0x8 | 0x4, 0x2, 0x8 | 0x2, 0x1, 0x8 | 0x1, 0x4 | 0x2, 0x8 | 0x4 | 0x2, 0x2 | 0x1, 0x8 | 0x2 | 0x1, 0x4 | 0x1, 0x8 | 0x4 | 0x1, 0x4 | 0x2 | 0x1, 0x8 | 0x4 | 0x2 | 0x1};

        /// <summary>Characters read by the ReadFile or ReadConsole function are written to the active screen buffer as they are read. This mode can be used only if the ENABLE_LINE_INPUT mode is also enabled.</summary>
        private static int ENABLE_ECHO_INPUT = 0x4;

        /// <summary>Holds the backcolor of the console window.</summary>
        private static ConsoleColor m_BackColor = ConsoleColor.Black;

        /// <summary>Holds the value of the CursorVisible property.</summary>
        private static bool m_CursorVisible = true;

        /// <summary>Holds the value of the EchoInput property.</summary>
        private static bool m_EchoInput = true;

        /// <summary>Holds the forecolor of the console window.</summary>
        private static ConsoleColor m_ForeColor = ConsoleColor.Gray;

        /// <summary>Holds the value of the OvrMode property.</summary>
        private static bool m_OvrMode;

        /// <summary>Standard input handle.</summary>
        private static int STD_INPUT_HANDLE = -10;

        /// <summary>Standard output handle.</summary>
        private static int STD_OUTPUT_HANDLE = -11;

        /// <summary>Gets or sets the color of the console font.</summary>
        /// <value>A value of the ConsoleColor enum that specifies the color of the console font.</value>
        public static ConsoleColor ForeColor
        {
            get { return m_ForeColor; }
            set
            {
                m_ForeColor = value;
                SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), ForegroundColors[(int) value] | BacgroundColors[(int) BackColor]);
            }
        }

        /// <summary>Gets or sets the color of the console background.</summary>
        /// <value>A value of the ConsoleColor enum that specifies the color of the console background.</value>
        public static ConsoleColor BackColor
        {
            get { return m_BackColor; }
            set
            {
                m_BackColor = value;
                SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), ForegroundColors[(int) ForeColor] | BacgroundColors[(int) value]);
            }
        }

        /// <summary>Gets or sets whether the cursor is visible or not.</summary>
        /// <value>A boolean value that specifies the visibility of the cursor.</value>
        public static bool CursorVisible
        {
            get { return m_CursorVisible; }
            set
            {
                m_CursorVisible = value;
                ChangeCursor();
            }
        }

        /// <summary>Gets or sets whether the cursor is in overwrite-mode or not.</summary>
        /// <value>A boolean value that specifies the mode of the cursor.</value>
        /// <remarks>In overwrite mode, the cursor size will be 50% of the character space instead of 25% in normal mode</remarks>
        public static bool OvrMode
        {
            get { return m_OvrMode; }
            set
            {
                m_OvrMode = value;
                ChangeCursor();
            }
        }

        /// <summary>Gets or sets whether the console must echo the input or not.</summary>
        /// <value>A boolean value that specifies the console must echo the input or not.</value>
        /// <remarks>EchoInput is often turned off when the program asks the user to type in a password.</remarks>
        public static bool EchoInput
        {
            get { return m_EchoInput; }
            set
            {
                m_EchoInput = value;
                int Ret = 0;
                GetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), ref Ret);
                if (EchoInput)
                {
                    Ret = Ret | ENABLE_ECHO_INPUT;
                }
                else
                {
                    Ret = Ret & ~ENABLE_ECHO_INPUT;
                }
                SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), Ret);
            }
        }

        /// <summary>Gets or sets the caption of the console.</summary>
        /// <value>A String that specifies the caption of the console.</value>
        public static string Caption
        {
            get
            {
                StringBuilder sb = new StringBuilder(256);
                GetConsoleTitle(sb, 256);
                return sb.ToString();
            }
            set { SetConsoleTitle(value); }
        }

        /// <summary>Gets or sets the current cursos position on the x axis in the console.</summary>
        /// <value>A short that specifies the current cursos position on the x axis in the console.</value>
        public static short CursorX
        {
            get
            {
                CONSOLE_SCREEN_BUFFER_INFO SBI = new CONSOLE_SCREEN_BUFFER_INFO();
                GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref SBI);
                return SBI.dwCursorPosition.x;
            }
            set { MoveCursor(value, CursorY); }
        }

        /// <summary>Gets or sets the current cursos position on the y axis in the console.</summary>
        /// <value>A short value that specifies the current cursos position on the y axis in the console.</value>
        public static short CursorY
        {
            get
            {
                CONSOLE_SCREEN_BUFFER_INFO SBI = new CONSOLE_SCREEN_BUFFER_INFO();
                GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref SBI);
                return SBI.dwCursorPosition.y;
            }
            set { MoveCursor(CursorX, value); }
        }

        /// <summary>Gets the width (in characters) of the console window.</summary>
        /// <value>An integer that holds the width of the console window in characters.</value>
        public static int WindowWidth
        {
            get
            {
                CONSOLE_SCREEN_BUFFER_INFO SBI = new CONSOLE_SCREEN_BUFFER_INFO();
                GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref SBI);
                return SBI.srWindow.Right - SBI.srWindow.Left + 1;
            }
        }

        /// <summary>Gets the height (in characters) of the console window.</summary>
        /// <value>An integer that holds the height of the console window in characters.</value>
        public static int WindowHeight
        {
            get
            {
                CONSOLE_SCREEN_BUFFER_INFO SBI = new CONSOLE_SCREEN_BUFFER_INFO();
                GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref SBI);
                return SBI.srWindow.Bottom - SBI.srWindow.Top + 1;
            }
        }

        /// <summary>
        /// The SetConsoleTextAttribute function sets the foreground (text) and background color attributes of characters written to the screen buffer by the WriteFile or WriteConsole function, or echoed by the ReadFile or ReadConsole function. This function affects only text written after the function call.
        /// </summary>
        /// <param name="hConsoleOutput">Handle to a console screen buffer. The handle must have GENERIC_READ access.</param>
        /// <param name="wAttributes">Specifies the foreground and background color attributes. Any combination of the following values can be specified: FOREGROUND_BLUE, FOREGROUND_GREEN, FOREGROUND_RED, FOREGROUND_INTENSITY, BACKGROUND_BLUE, BACKGROUND_GREEN, BACKGROUND_RED, and BACKGROUND_INTENSITY.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "SetConsoleTextAttribute", CharSet = CharSet.Ansi)]
        internal static extern int SetConsoleTextAttribute(int hConsoleOutput, int wAttributes);

        /// <summary>
        /// The GetStdHandle function returns a handle for the standard input, standard output, or standard error device.
        /// </summary>
        /// <param name="nStdHandle">Specifies the device for which to return the handle. This parameter can have one of the following values:
        /// <list type="bullet"> 
        ///		<listheader>
        ///			<value>Value</value>
        ///			<meaning>Meaning</meaning>
        ///		</listheader>
        ///		<item>
        ///			<value>STD_INPUT_HANDLE</value>
        ///			<meaning>Standard input handle.</meaning>
        ///		</item>
        ///		<item>
        ///			<value>STD_OUTPUT_HANDLE</value>
        ///			<meaning>Standard output handle.</meaning>
        ///		</item>
        ///		<item>
        ///			<value>STD_ERROR_HANDLE</value>
        ///			<meaning>Standard error handle.</meaning>
        ///		</item>
        /// </list>
        /// </param>
        /// <returns>If the function succeeds, the return value is a handle to the specified device.<br></br><br>If the function fails, the return value is the INVALID_HANDLE_VALUE flag. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetStdHandle")]
        internal static extern int GetStdHandle(int nStdHandle);

        /// <summary>
        /// The SetConsoleCursorInfo function sets the size and visibility of the cursor for the specified console screen buffer.
        /// </summary>
        /// <param name="hConsoleOutput">Handle to a console screen buffer. The handle must have GENERIC_WRITE access.</param>
        /// <param name="lpConsoleCursorInfo">Pointer to a CONSOLE_CURSOR_INFO structure containing the new specifications for the screen buffer's cursor.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "SetConsoleCursorInfo")]
        internal static extern int SetConsoleCursorInfo(int hConsoleOutput, ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

        /// <summary>
        /// The GetConsoleMode function reports the current input mode of a console's input buffer or the current output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleHandle">Handle to a console input buffer or a screen buffer. The handle must have GENERIC_READ access.</param>
        /// <param name="lpConsoleCursorInfo">
        /// Pointer to a 32-bit variable that indicates the current mode of the specified buffer.<br>If the hConsoleHandle parameter is an input handle, the mode can be a combination of the following values. When a console is created, all input modes except ENABLE_WINDOW_INPUT are enabled by default.</br>
        /// <list type="bullet">
        /// 	<listheader>
        /// 		<value>Value</value>
        /// 		<meaning>Meaning</meaning>
        /// 	</listheader>
        /// 	<item>
        /// 		<value>ENABLE_LINE_INPUT</value>
        /// 		<meaning>The ReadFile or ReadConsole function returns only when a carriage return character is read. If this mode is disabled, the functions return when one or more characters are available.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_ECHO_INPUT</value>
        /// 		<meaning>Characters read by the ReadFile or ReadConsole function are written to the active screen buffer as they are read. This mode can be used only if the ENABLE_LINE_INPUT mode is also enabled.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_PROCESSED_INPUT</value>
        /// 		<meaning>ctrl+c is processed by the system and is not placed in the input buffer. If the input buffer is being read by ReadFile or ReadConsole, other control keys are processed by the system and are not returned in the ReadFile or ReadConsole buffer. If the ENABLE_LINE_INPUT mode is also enabled, backspace, carriage return, and linefeed characters are handled by the system.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_WINDOW_INPUT</value>
        /// 		<meaning>User interactions that change the size of the console screen buffer are reported in the console's input buffer. Information about these events can be read from the input buffer by applications using the ReadConsoleInput function, but not by those using ReadFile or ReadConsole.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_MOUSE_INPUT</value>
        /// 		<meaning>If the mouse pointer is within the borders of the console window and the window has the keyboard focus, mouse events generated by mouse movement and button presses are placed in the input buffer. These events are discarded by ReadFile or ReadConsole, even when this mode is enabled.</meaning>
        /// 	</item>
        /// </list>
        /// If the hConsoleHandle parameter is a screen buffer handle, the mode can be a combination of the following values. When a screen buffer is created, both output modes are enabled by default.
        /// <list type="bullet">
        /// 	<listheader>
        /// 		<value>Value</value>
        /// 		<meaning>Meaning</meaning>
        /// 	</listheader>
        /// 	<item>
        /// 		<value>ENABLE_PROCESSED_OUTPUT</value>
        /// 		<meaning>Characters written by the WriteFile or WriteConsole function or echoed by the ReadFile or ReadConsole function are parsed for ASCII control sequences, and the correct action is performed. Backspace, tab, bell, carriage return, and linefeed characters are processed.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_WRAP_AT_EOL_OUTPUT</value>
        /// 		<meaning>When writing with WriteFile or WriteConsole or echoing with ReadFile or ReadConsole, the cursor moves to the beginning of the next row when it reaches the end of the current row. This causes the rows displayed in the console window to scroll up automatically when the cursor advances beyond the last row in the window. It also causes the contents of the screen buffer to scroll up (discarding the top row of the screen buffer) when the cursor advances beyond the last row in the screen buffer. If this mode is disabled, the last character in the row is overwritten with any subsequent characters.</meaning>
        /// 	</item>
        /// </list>
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetConsoleMode")]
        internal static extern int GetConsoleMode(int hConsoleHandle, ref int lpConsoleCursorInfo);

        /// <summary>
        /// The SetConsoleMode function sets the input mode of a console's input buffer or the output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleHandle">Handle to a console input buffer or a screen buffer. The handle must have GENERIC_WRITE access.</param>
        /// <param name="lpConsoleCursorInfo">
        /// Pointer to a 32-bit variable that indicates the current mode of the specified buffer.<br>If the hConsoleHandle parameter is an input handle, the mode can be a combination of the following values. When a console is created, all input modes except ENABLE_WINDOW_INPUT are enabled by default.</br>
        /// <list type="bullet">
        /// 	<listheader>
        /// 		<value>Value</value>
        /// 		<meaning>Meaning</meaning>
        /// 	</listheader>
        /// 	<item>
        /// 		<value>ENABLE_LINE_INPUT</value>
        /// 		<meaning>The ReadFile or ReadConsole function returns only when a carriage return character is read. If this mode is disabled, the functions return when one or more characters are available.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_ECHO_INPUT</value>
        /// 		<meaning>Characters read by the ReadFile or ReadConsole function are written to the active screen buffer as they are read. This mode can be used only if the ENABLE_LINE_INPUT mode is also enabled.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_PROCESSED_INPUT</value>
        /// 		<meaning>ctrl+c is processed by the system and is not placed in the input buffer. If the input buffer is being read by ReadFile or ReadConsole, other control keys are processed by the system and are not returned in the ReadFile or ReadConsole buffer. If the ENABLE_LINE_INPUT mode is also enabled, backspace, carriage return, and linefeed characters are handled by the system.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_WINDOW_INPUT</value>
        /// 		<meaning>User interactions that change the size of the console screen buffer are reported in the console's input buffer. Information about these events can be read from the input buffer by applications using the ReadConsoleInput function, but not by those using ReadFile or ReadConsole.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_MOUSE_INPUT</value>
        /// 		<meaning>If the mouse pointer is within the borders of the console window and the window has the keyboard focus, mouse events generated by mouse movement and button presses are placed in the input buffer. These events are discarded by ReadFile or ReadConsole, even when this mode is enabled.</meaning>
        /// 	</item>
        /// </list>
        /// If the hConsoleHandle parameter is a screen buffer handle, the mode can be a combination of the following values. When a screen buffer is created, both output modes are enabled by default.
        /// <list type="bullet">
        /// 	<listheader>
        /// 		<value>Value</value>
        /// 		<meaning>Meaning</meaning>
        /// 	</listheader>
        /// 	<item>
        /// 		<value>ENABLE_PROCESSED_OUTPUT</value>
        /// 		<meaning>Characters written by the WriteFile or WriteConsole function or echoed by the ReadFile or ReadConsole function are parsed for ASCII control sequences, and the correct action is performed. Backspace, tab, bell, carriage return, and linefeed characters are processed.</meaning>
        /// 	</item>
        /// 	<item>
        /// 		<value>ENABLE_WRAP_AT_EOL_OUTPUT</value>
        /// 		<meaning>When writing with WriteFile or WriteConsole or echoing with ReadFile or ReadConsole, the cursor moves to the beginning of the next row when it reaches the end of the current row. This causes the rows displayed in the console window to scroll up automatically when the cursor advances beyond the last row in the window. It also causes the contents of the screen buffer to scroll up (discarding the top row of the screen buffer) when the cursor advances beyond the last row in the screen buffer. If this mode is disabled, the last character in the row is overwritten with any subsequent characters.</meaning>
        /// 	</item>
        /// </list>
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "SetConsoleMode")]
        internal static extern int SetConsoleMode(int hConsoleHandle, int lpConsoleCursorInfo);

        /// <summary>
        /// The SetConsoleTitle function sets the title bar string for the current console window.
        /// </summary>
        /// <param name="lpConsoleTitle">Pointer to a null-terminated string that contains the string to appear in the title bar of the console window.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "SetConsoleTitleA", CharSet = CharSet.Ansi)]
        internal static extern int SetConsoleTitle(string lpConsoleTitle);

        /// <summary>
        /// The GetConsoleTitle function retrieves the title bar string for the current console window.
        /// </summary>
        /// <param name="lpConsoleTitle">Pointer to a buffer that receives a null-terminated string containing the text that appears in the title bar of the console window.</param>
        /// <param name="nSize">Specifies the size, in characters, of the buffer pointed to by the lpConsoleTitle parameter.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the string copied to the buffer.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetConsoleTitleA", CharSet = CharSet.Ansi)]
        internal static extern int GetConsoleTitle(StringBuilder lpConsoleTitle, int nSize);

        /// <summary>
        /// The GetConsoleScreenBufferInfo function retrieves information about the specified console screen buffer.
        /// </summary>
        /// <param name="hConsoleOutput">Handle to a console screen buffer. The handle must have GENERIC_READ access.</param>
        /// <param name="lpConsoleScreenBufferInfo">Pointer to a CONSOLE_SCREEN_BUFFER_INFO structure in which the screen buffer information is returned.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetConsoleScreenBufferInfo")]
        internal static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        /// <summary>
        /// The SetConsoleCursorPosition function sets the cursor position in the specified console screen buffer.
        /// </summary>
        /// <param name="hConsoleOutput">Handle to a console screen buffer. The handle must have GENERIC_WRITE access.</param>
        /// <param name="dwCursorPosition">Specifies a COORD structure containing the new cursor position. The coordinates are the column and row of a screen buffer character cell. The coordinates must be within the boundaries of the screen buffer.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call GetLastError.</br></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "SetConsoleCursorPosition")]
        internal static extern int SetConsoleCursorPosition(int hConsoleOutput, ref COORD dwCursorPosition);

        /// <summary>Applies the current cursor settings.</summary>
        /// <remarks>This method applies changes in the CursorVisible and OvrMode properties.</remarks>
        private static void ChangeCursor()
        {
            CONSOLE_CURSOR_INFO CCI;
            CCI.bVisible = Convert.ToInt32(CursorVisible);
            CCI.dwSize = (OvrMode ? 50 : 25);
            SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref CCI);
        }

        /// <summary>Moves the cursor to the specified location.</summary>
        /// <param name="x">Specifies the x value of the new location.</param>
        /// <param name="y">Specifies the y value of the new location.</param>
        public static void MoveCursor(short x, short y)
        {
            COORD Crd;
            Crd.x = x;
            Crd.y = y;
            SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), ref Crd);
        }
    }
}