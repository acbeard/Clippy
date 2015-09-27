using System;
using ClippyLib;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UT.ClippyLib.Editors
{
	public abstract class AEditorTester
	{
		string actual;
		string contents;
		string editorResponse;
		string persistentEditorResponse;

		protected void WhenClipboardContains(string contents)
		{
			this.contents = contents;
		}

		protected void RegardlessOfClipboardContent()
		{
		}

		protected void WhenCommandIsRan(string editorWithCommands)
		{
			AndCommandIsRan(editorWithCommands);
		}

		protected void AndCommandIsRan(string editorWithCommands)
		{
			EditorManager manager = new EditorManager();
			string[] args = manager.GetArgumentsFromString(editorWithCommands);
			IClipEditor editor = manager.GetClipEditor(args[0]);
			manager.ClipEditor.EditorResponse += (a,b) => {editorResponse = b.ResponseString;};
			manager.ClipEditor.PersistentEditorResponse += (a,b) => {persistentEditorResponse = b.ResponseString;};

			string[] realArgs = new string[args.Length-1];
			for(int i=0;i<realArgs.Length;i++)
			{
				realArgs[i] = args[i+1];
			}

			actual = TestEditor(editor, this.contents, realArgs);
		}

		protected void ThenTheClipboardShouldContain(string expected)
		{
			Assert.AreEqual(expected, this.actual);
		}

		protected void ThenTheClipboardShouldContainSubstring(string substringExpected)
		{
			Assert.IsTrue(this.actual.Contains(substringExpected));
		}

		protected void ThenClipboardShouldMatchRegex(string pattern)
		{
			Assert.IsTrue(Regex.IsMatch(this.actual, pattern, RegexOptions.IgnoreCase));
		}

		protected void ThenClippyShouldRespondWith(string expectedResponse)
		{
			Assert.AreEqual(expectedResponse, this.editorResponse);
		}

		protected void ThenClippyShouldRespondAndStayOpenWithMessage(string expectedResponse)
		{
			Assert.AreEqual(expectedResponse, this.persistentEditorResponse);
		}

		protected void AndSourceDataShouldNotHaveChanged()
		{
			Assert.AreEqual(contents, actual);
		}

		private static string TestEditor(IClipEditor editor, string input)
		{
			return TestEditor(editor, input, new string[0]);
		}

		private static string TestEditor(IClipEditor editor, string input, string command)
		{
			return TestEditor(editor, input, new []{command});
		}

		protected static string TestEditor(IClipEditor editor, string input, params string[] commands)
		{
			editor.SourceData = input;
			editor.DefineParameters();
			for(int i=0;i<commands.Length;i++)
			{
				editor.SetNextParameter(commands[i]);
			}
			editor.Edit();
			return editor.SourceData;
		}

		private static void AssertEditor(string expected, IClipEditor editor, string input, params string[] commands)
		{
			string actual = TestEditor(editor, input, commands);
			Assert.AreEqual(expected, actual);
		}

	}
}

