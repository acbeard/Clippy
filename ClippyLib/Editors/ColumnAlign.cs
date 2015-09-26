﻿/*
 * 
 * Copyright 2012-2015 Matthew Rikard
 * This file is part of Clippy.
 * 
 *  Clippy is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Clippy is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Clippy.  If not, see <http://www.gnu.org/licenses/>.
 *
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClippyLib.Editors
{
    public class ColumnAlign : AClipEditor
    {
		
		private Regex _colSplitter;
		private List<int> _columnLengths;

        public override string EditorName
        {
            get { return "ColumnAlign"; }
        }

        public override string ShortDescription
        {
            get { return "Takes delimited text and transforms it to fixed width."; }
        }

        public override string LongDescription
        {
            get
            {
                return @"ColumnAlign
Syntax: clippy columnAlign [numberOfSpaces] [originalDelimiter]

Takes delimited data (such as from a grid, or csv) and converts it to line up the columns
when printed with a fixed width font.

numberOfSpaces - a number defining the number of spaces between each column.
Defaults to 2

originalDelimiter - a string delimiter of the original text.
for instance for csv use "",""
Defaults to tab character

Example:
    clippy columnalign 3 \t
    will align tab delimited text with 3 spaces between each column.
";
            }
        }

        private bool IsPositiveInteger(string n)
        {
            short s;
            if (Int16.TryParse(n, out s))
            {
                if (s >= 0)
                    return true;
            }
            return false;
        }

        public override void DefineParameters()
        {
            _parameterList = new List<Parameter>();
            _parameterList.Add(new Parameter()
            {
                ParameterName = "Number of spaces between columns",
                Sequence = 1,
                Validator = IsPositiveInteger,
                DefaultValue = "2",
                Required = false,
                Expecting = "an integer (16 bit) greater than zero"
            });
            _parameterList.Add(new Parameter()
            {
                ParameterName = "Original delimiter",
                Sequence = 2,
                Validator = a => true,
                DefaultValue = "\t",
                Required = false,
                Expecting = "an string delimiter"
            });
        }


        public override void SetParameters(string[] args)
        {
            for(int i=0;i<ParameterList.Count;i++)
                ParameterList[i].Value = ParameterList[i].DefaultValue;
            if (args.Length > 1)
                SetParameter(1, args[1]);
            if (args.Length > 2)
                SetParameter(2, args[2]);
        }


        public override void Edit()
        {
			_colSplitter = new Regex(ParameterList[1].GetEscapedValueOrDefault(), RegexOptions.IgnoreCase);

			//todo: \r should be accounted for in each of the IClipEditors
            string[] rows = SourceData.Split('\n');
            
            
            DefineColumnLengths (rows);
			RebuildRows (rows);
            SourceData = String.Join("\n", rows);
        }       
        
		private void DefineColumnLengths (string[] rows)
		{
			_columnLengths = new List<int>();
			for (int r = 0; r < rows.Length; r++) 
			{
				string[] cols = _colSplitter.Split (rows [r]);
				for (int c = 0; c < cols.Length; c++) 
				{
					if (c >= _columnLengths.Count)
					{
						_columnLengths.Add(cols[c].Length);
					}
					_columnLengths[c] = Math.Max(_columnLengths[c], cols[c].Length);
				}
			}
		}

		private void RebuildRows (string[] rows)
		{
			string columnSeparator = new System.String(' ', Int32.Parse (ParameterList[0].GetValueOrDefault()));

			for (int r = 0; r < rows.Length; r++) 
			{
				string[] cols = _colSplitter.Split(rows[r]);
				for (int c = 0; c < cols.Length; c++) 
				{
					cols[c] = cols[c].PadRight(_columnLengths[c],' ');
				}
				rows[r] = String.Join(columnSeparator, cols).Trim();
			}
		}
    }
}
