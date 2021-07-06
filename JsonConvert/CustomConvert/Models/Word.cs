using System;
using System.Collections.Generic;
using System.Text;

namespace JsonConvert
{
    class Word
    {
        public string Value { get; private set; }
        public int StartPosition { get; private set; }
        public int EndPosition { get; private set; }
        public int Level { get; set; }
        public bool IsProperty { get; set; }
        public Word Father { get; set; }
        public bool HaveValue { get; set; }

        public Word(string value, int start, int end)
        {
            Value = value;
            StartPosition = start;
            EndPosition = end;
        }



    }
}
