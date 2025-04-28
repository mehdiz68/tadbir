using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreLib.Infrastructure.Captcha
{
    public class zCaptcha
    {
        public string NumberOne { get; set; }
        public int Number1 { get; set; }
        public string CurrentNumber1 { get; set; }

        public string NumberTwo { get; set; }
        public int Number2 { get; set; }
        public string CurrentNumber2 { get; set; }

        public string NumberThree { get; set; }
        public int Number3 { get; set; }
        public string CurrentNumber3 { get; set; }

        public string Operator { get; set; }
        public int AnswerType { get; set; }

        public zCaptcha()
        {
            Random r = new Random();
            //operator
            int numberType = r.Next(1, 3);
            if (numberType == 1)
                Operator = "+";
            else
                Operator = "-";

            //answer show type
            AnswerType = r.Next(1, 4);
             numberType = 0;

            #region Number 1 Is Question
            if (AnswerType==1)
            {
                //number 2
                numberType = r.Next(1, 3);
                Number2 = r.Next(1, 9);
                NumberTwo = GetRandomPersianAlphabet(Number2).ToString();
                if (numberType == 1)
                    CurrentNumber2 = Number2.ToString();
                else
                    CurrentNumber2 = GetRandomPersianAlphabet(Number2).ToString();

                //number 3
                numberType = r.Next(1, 3);
                Number3 = r.Next(1, 9);
                NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                if (numberType == 1)
                    CurrentNumber3 = Number3.ToString();
                else
                    CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();

                if (Operator == "+")
                {
                    while (Number3 < Number2)
                    {
                        //number 2
                        Number2 = r.Next(1, 9);
                        NumberTwo = GetRandomPersianAlphabet(Number2).ToString();
                        if (numberType == 1)
                            CurrentNumber2 = Number2.ToString();
                        else
                            CurrentNumber2 = GetRandomPersianAlphabet(Number2).ToString();

                        //number 3
                        Number3 = r.Next(1, 9);
                        NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                        if (numberType == 1)
                            CurrentNumber3 = Number3.ToString();
                        else
                            CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();


                    }
                }
                else
                {
                    while (Number3 > Number2)
                    {
                        //number 2
                        Number2 = r.Next(1, 9);
                        NumberTwo = GetRandomPersianAlphabet(Number2).ToString();
                        if (numberType == 1)
                            CurrentNumber2 = Number2.ToString();
                        else
                            CurrentNumber2 = GetRandomPersianAlphabet(Number2).ToString();

                        //number 3
                        Number3 = r.Next(1, 9);
                        NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                        if (numberType == 1)
                            CurrentNumber3 = Number3.ToString();
                        else
                            CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();


                    }
                }

                if (Operator == "+")
                    Number1 = Number3 - Number2;
                else 
                    Number1 = Number3 + Number2;


            }
            #endregion

            #region Number 2 Is Question
            if (AnswerType == 2)
            {
                //number 1
                numberType = r.Next(1, 3);
                Number1 = r.Next(1, 9);
                NumberOne = GetRandomPersianAlphabet(Number1).ToString();
                if (numberType == 1)
                    CurrentNumber1 = Number1.ToString();
                else
                    CurrentNumber1 = GetRandomPersianAlphabet(Number1).ToString();

                //number 3
                numberType = r.Next(1, 3);
                Number3 = r.Next(1, 9);
                NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                if (numberType == 1)
                    CurrentNumber3 = Number3.ToString();
                else
                    CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();

                if (Operator == "+")
                {
                    while (Number3 < Number1)
                    {
                        //number 1
                        Number1 = r.Next(1, 9);
                        NumberOne = GetRandomPersianAlphabet(Number1).ToString();
                        if (numberType == 1)
                            CurrentNumber1 = Number1.ToString();
                        else
                            CurrentNumber1 = GetRandomPersianAlphabet(Number1).ToString();

                        //number 3
                        Number3 = r.Next(1, 9);
                        NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                        if (numberType == 1)
                            CurrentNumber3 = Number3.ToString();
                        else
                            CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();


                    }
                }
                else
                {
                    while (Number3 > Number1)
                    {
                        //number 2
                        Number1 = r.Next(1, 9);
                        NumberOne = GetRandomPersianAlphabet(Number1).ToString();
                        if (numberType == 1)
                            CurrentNumber1 = Number1.ToString();
                        else
                            CurrentNumber1 = GetRandomPersianAlphabet(Number1).ToString();

                        //number 3
                        Number3 = r.Next(1, 9);
                        NumberThree = GetRandomPersianAlphabet(Number3).ToString();
                        if (numberType == 1)
                            CurrentNumber3 = Number3.ToString();
                        else
                            CurrentNumber3 = GetRandomPersianAlphabet(Number3).ToString();


                    }
                }

                if (Operator == "+")
                    Number2 = Number3 - Number1;
                else
                    Number2 = Number1 - Number3;


            }
            #endregion

            #region Number 3 Is Question
            if (AnswerType == 3)
            {
                //number 1
                numberType = r.Next(1, 3);
                Number1 = r.Next(1, 9);
                NumberOne = GetRandomPersianAlphabet(Number1).ToString();
                if (numberType == 1)
                    CurrentNumber1 = Number1.ToString();
                else
                    CurrentNumber1 = GetRandomPersianAlphabet(Number1).ToString();

                //number 2
                numberType = r.Next(1, 3);
                Number2 = r.Next(1, 9);
                NumberTwo = GetRandomPersianAlphabet(Number2).ToString();
                if (numberType == 1)
                    CurrentNumber2 = Number2.ToString();
                else
                    CurrentNumber2 = GetRandomPersianAlphabet(Number2).ToString();

               
                if(Operator=="-")
                {
                    while (Number2 > Number1)
                    {
                        //number 1
                        numberType = r.Next(1, 3);
                        Number1 = r.Next(1, 9);
                        NumberOne = GetRandomPersianAlphabet(Number1).ToString();
                        if (numberType == 1)
                            CurrentNumber1 = Number1.ToString();
                        else
                            CurrentNumber1 = GetRandomPersianAlphabet(Number1).ToString();

                        //number 2
                        numberType = r.Next(1, 3);
                        Number2 = r.Next(1, 9);
                        NumberTwo = GetRandomPersianAlphabet(Number2).ToString();
                        if (numberType == 1)
                            CurrentNumber2 = Number2.ToString();
                        else
                            CurrentNumber2 = GetRandomPersianAlphabet(Number2).ToString();


                    }
                }

                if (Operator == "+")
                    Number3 = Number1 + Number2;
                else
                    Number3 = Number1 - Number2;


            }
            #endregion

            
        }

        private string GetRandomPersianAlphabet(int number)
        {
            switch (number)
            {
                case 1:return "1";
                case 2: return "2";
                case 3: return "3";
                case 4: return "4";
                case 5: return "5";
                case 6: return "6";
                case 7: return "7";
                case 8: return "8";
                case 9: return "9";
                //case 10: return "ده";
                //case 11: return "یازده";
                //case 12: return "دوازده";
                //case 13: return "سیزده";
                //case 14: return "چهارده";
                //case 15: return "پانزده";
                //case 16: return "شانزده";
                //case 17: return "هفده";
                //case 18: return "هجده";
                //case 19: return "نوزده";
                //case 20: return "بیست";
                //case 21: return "بیست و یک";
                //case 22: return "بیست و دو";
                //case 23: return "بیست و سه";
                //case 24: return "بیست و چهار";
                //case 25: return "بیست و پنج";
                //case 26: return "بیست و شش";
                //case 27: return "بیست و هفت";
                //case 28: return "بیست و هشت";
                //case 29: return "بیست و نه";
                //case 30: return "سی";
                //case 31: return "سی و یک";
                //case 32: return "سی و دو";
                //case 33: return "سی و سه";
                //case 34: return "سی و چهار";
                //case 35: return "سی و پنج";
                //case 36: return "سی و شش";
                //case 37: return "سی و هفت";
                //case 38: return "سی و هشت";
                //case 39: return "سی و نه";
                //case 40: return "چهل";
                //case 41: return "چهل و یک";
                //case 42: return "چهل و دو";
                //case 43: return "چهل و سه";
                //case 44: return "چهل و چهار";
                //case 45: return "چهل و پنج";
                //case 46: return "چهل و شش";
                //case 47: return "چهل و هفت";
                //case 48: return "چهل و هشت";
                //case 49: return "چهل و نه";
                //case 50: return "پنجاه";
                //case 51: return "پنجاه و یک";
                //case 52: return "پنجاه و دو";
                //case 53: return "پنجاه و سه";
                //case 54: return "پنجاه و چهار";
                //case 55: return "پنجاه و پنج";
                //case 56: return "پنجاه و شش";
                //case 57: return "پنجاه و هفت";
                //case 58: return "پنجاه و هشت";
                //case 59: return "پنجاه و نه";
                //case 60: return "شصت";
                //case 61: return "شصت و یک";
                //case 62: return "شصت و دو";
                //case 63: return "شصت و سه";
                //case 64: return "شصت و چهار";
                //case 65: return "شصت و پنج";
                //case 66: return "شصت و شش";
                //case 67: return "شصت و هفت";
                //case 68: return "شصت و هشت";
                //case 69: return "شصت و نه";
                //case 70: return "هفتاد";
                //case 71: return "هفتاد و یک";
                //case 72: return "هفتاد و دو";
                //case 73: return "هفتاد و سه";
                //case 74: return "هفتاد و چهار";
                //case 75: return "هفتاد و پنج";
                //case 76: return "هفتاد و شش";
                //case 77: return "هفتاد و هفت";
                //case 78: return "هفتاد و هشت";
                //case 79: return "هفتاد و نه";
                //case 80: return "هشتاد";
                //case 81: return "هشتاد و یک";
                //case 82: return "هشتاد و دو";
                //case 83: return "هشتاد و سه";
                //case 84: return "هشتاد و چهار";
                //case 85: return "هشتاد و پنج";
                //case 86: return "هشتاد و شش";
                //case 87: return "هشتاد و هفت";
                //case 88: return "هشتاد و هشت";
                //case 89: return "هشتاد و نه";
                //case 90: return "نود";
                //case 91: return "نود و یک";
                //case 92: return "نود و دو";
                //case 93: return "نود و سه";
                //case 94: return "نود و چهار";
                //case 95: return "نود و پنج";
                //case 96: return "نود و شش";
                //case 97: return "نود و هفت";
                //case 98: return "نود و هشت";
                //case 99: return "نود و نه";
                //case 100: return "صد";
                default:return "1";
            }
        }

        public int GetCurrentValue()
        {
            switch (AnswerType)
            {
                case 1:return Number1;
                case 2: return Number2;
                case 3: return Number3;
                default: return 0;
            }
        }
      

    }
}