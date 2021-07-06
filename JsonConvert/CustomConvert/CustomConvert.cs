using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

    /* Якщо, що я пробував робити за допомогоб всяких різних ліб, але все воно в мене трошки виходило з-під контролю на великих рівнях вкладеності,
     В цьому варіанті, поки все, що я тестив проходило, хоч і костилів тут вистачає. Не судіть суворо
    */
    
namespace JsonConvert
{
    class CustomConvert
    {

        private readonly string text;
        private readonly List<Word> listOfWord;

        public CustomConvert(string filePath)
        {
            listOfWord = new List<Word>();

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException();
            }

            using (StreamReader streamreader = new StreamReader(filePath))
            {
                text = streamreader.ReadToEnd();
            }
        }


        public Dictionary<string, string> ConvertToDictionary()
        { 
           
            var result = new Dictionary<string, string>();
            try
            {

                DuplicatePropertyNameHandling duplicateFlag = 0;
                JsonLoadSettings jsonLoadSettings = (duplicateFlag == DuplicatePropertyNameHandling.Replace)
                    ? null
                    : new JsonLoadSettings { DuplicatePropertyNameHandling = duplicateFlag };
               
                JObject.Parse(text, jsonLoadSettings);

                GetWords();

                var propertyTree = GetFatherTree();
                var keyArr = listOfWord.Where(x => x.IsProperty == false).Select(x => x.Value).ToArray();

                for (int i = 0; i < keyArr.Length; i++)
                {
                    result.Add(propertyTree[i], keyArr[i]);
                }
                return result;
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("JSON isn't Valid");
                return null;
            }
            catch (Exception)
            {
                Console.WriteLine("JSON have duplicate keys");
                return null;
            }
        }

        private void GetWords()
        {
            var listOfWordPosition = new List<int>();
            var isNotStringValue = false;

            for (int i = 0; i < text.Length; i++)
            {
               
                if (text[i] == '"' && isNotStringValue)
                {
                    listOfWordPosition.Remove(listOfWordPosition.Last());
                }

                if (text[i] == '"' && text[i - 1] != '\\')
                {

                    isNotStringValue = false;
                    listOfWordPosition.Add(i);
                }

                if (text[i] == ':' && !isNotStringValue)
                {
                    listOfWordPosition.Add(i);
                    isNotStringValue = true;
                }

                if (text[i] == '{' && isNotStringValue)
                {
                    listOfWordPosition.Remove(listOfWordPosition.Last());
                    isNotStringValue = false;
                }

                if (isNotStringValue && (text[i] == '}'|| text[i] == '\n'|| text[i] == '\r'||text[i] == '\t'|| text[i]==','))
                {
                    listOfWordPosition.Add(i);
                    isNotStringValue = false;
                }
            }
            
            for (int i = 0; i < listOfWordPosition.Count; i = i + 2)
            {
                var end = listOfWordPosition[i + 1];
                var start = listOfWordPosition[i];
                var length = listOfWordPosition[i + 1] - listOfWordPosition[i]-1;
                var word = String.Join("", text.ToCharArray().Skip(listOfWordPosition[i] + 1).Take(length));
                
                listOfWord.Add(new Word(word,start,end));
                
            }

            IsProperty();
            HaveValue();
            GetLevels();
            SetFather();
        }

        #region Level
        private void GetLevels()
        {
            var level = 0;
            var listOfFirstIndexOfWord = listOfWord.Select(x => x.StartPosition).ToArray();
            var index = 0;
            for (int i = 1; i < listOfFirstIndexOfWord.Last()+1; i++)
            {
              
                if (text[i] == '{')
                {
                    level++;
                }

                if (text[i] == '}')
                {
                    level--;
                }

                if (i == listOfFirstIndexOfWord[index])
                {
                    listOfWord[index].Level = level;
                    index++;
                }
            }
        }

        #endregion

        private void IsProperty()
        {
            listOfWord.First().IsProperty = true;
            listOfWord.Last().IsProperty = false;

            for (int i = 1; i < listOfWord.Count-1; i++)
            {
                var item = listOfWord[i];
                var nextItem = listOfWord[i + 1];

                var start = item.EndPosition;
                var end = nextItem.StartPosition+1;

                for (int s = start; s < end; s++)
                {
                    if (text[s] == ':')
                    {
                        item.IsProperty = true;
                        break;
                    }
                        
                }

            }
        }

        private void HaveValue()
        {
            for (int i = 0; i < listOfWord.Count-1; i++)
            {
                if (listOfWord[i + 1].IsProperty == false)
                {
                    listOfWord[i].HaveValue = true;
                }
            }
        }

        private void SetFather()
        {
            foreach (var word in listOfWord)
            {
                if (word.Level != 0 && word.IsProperty)
                {
                    word.Father = listOfWord.Where(x => x.Level == word.Level - 1 && x.EndPosition < word.EndPosition).Last();
                }
            }
        }

        private string[] GetFatherTree()
        {
            var listOfValue = listOfWord.Select(x => x).Where(x => x.IsProperty == false).ToArray();
            var resultArray = new string[listOfValue.Length];
            var index = 0;
            foreach (var item in listOfValue)
            {
                var resultTree = "";
                var property = listOfWord.Where(x => x.IsProperty == true && x.EndPosition < item.EndPosition).Last();
                var level = property.Level;
                resultTree += property.Value + " = ";
                while (level != 0)
                {
                    level--;
                    var substring = property.Father.Value + ":";
                    property = property.Father;
                    resultTree = substring + resultTree;
                } ;

                resultArray[index] = resultTree;
                index++;
            }

            return resultArray;
        }

    }
}
