using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ProjectEternity.Core.AI
{
    public sealed class CoreScriptHolder : CoreAI
    {
        public override KeyValuePair<string, List<AIScript>> GetNameAndContent(params object[] args)
        {
            List<AIScript> ListAIScript = ReflectionHelper.GetNestedTypes<AIScript>(typeof(CoreScriptHolder), args);
            return new KeyValuePair<string, List<AIScript>>("Core", ListAIScript);
        }

        public class OnStep : ScriptEvent
        {
            public OnStep()
                : base(100, 50, "On Step", new string[1] { "Event Called" }, new string[0])
            {
            }

            public override void OnCalled(GameTime gameTime, out List<object> Result)
            {
                bool IsCompleted = false;
                ExecuteFollowingScripts(0, gameTime, null, out IsCompleted, out Result);
            }
            
            public override AIScript CopyScript()
            {
                return new OnStep();
            }
        }

        public class OnHit : ScriptEvent
        {
            public OnHit()
                : base(100, 50, "On Hit", new string[1] { "Event Called" }, new string[0])
            {
            }

            public override void OnCalled(GameTime gameTime, out List<object> Result)
            {
                bool IsCompleted = false;
                ExecuteFollowingScripts(0, gameTime, Name, out IsCompleted, out Result);
            }

            public override AIScript CopyScript()
            {
                return new OnHit();
            }
        }

        /// <summary>
        /// Calls a group of scripts.
        /// </summary>
        public class Subroutine : AIScript, ScriptEvaluator
        {
            private string _ScriptToUsePath;
            private AIContainer SubRoutineToUse;

            public Subroutine()
                : base(150, 50, "Subroutine", new string[1] { "Subroutine finished" }, new string[0])
            {
                _ScriptToUsePath = string.Empty;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                IsCompleted = false;
                Result = null;
                if (Input != null)
                {
                    string InputEvent = (string)Input;
                    SubRoutineToUse.Update(gameTime, InputEvent);
                }
                else
                {
                    SubRoutineToUse.UpdateStep(gameTime);
                }

                if (SubRoutineToUse.ListReturnValue.Count > 0)
                {
                    ExecuteFollowingScripts(0, gameTime, SubRoutineToUse.ListReturnValue, out IsCompleted, out Result);
                }
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _ScriptToUsePath = BR.ReadString();

                if (Owner != null)
                {
                    SubRoutineToUse = Owner.Copy();

                    if (File.Exists("Content/AIs/" + _ScriptToUsePath + ".peai"))
                        SubRoutineToUse.Load(_ScriptToUsePath);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_ScriptToUsePath);
            }

            public override AIScript CopyScript()
            {
                return new Subroutine();
            }

            [Editor(typeof(Selectors.AISelector), typeof(UITypeEditor)),
            CategoryAttribute("Script Attributes"),
            DescriptionAttribute("The AI path"),
            DefaultValueAttribute("")]
            public string ScriptToUsePath
            {
                get
                {
                    return _ScriptToUsePath;
                }
                set
                {
                    _ScriptToUsePath = value;
                }
            }
        }

        /// <summary>
        /// Acts like a SubRoutine but automatically call the contained script by itself.
        /// </summary>
        public class CustomEvent : ScriptEvent
        {
            private string _ScriptToUsePath;
            private AIContainer EventToUse;

            public CustomEvent()
                : base(150, 50, "Custom Event", new string[1] { "Event Called" }, new string[0])
            {
            }

            public override void OnCalled(GameTime gameTime, out List<object> Result)
            {
                bool IsCompleted = false;
                Result = null;

                EventToUse.UpdateStep(gameTime);

                if (EventToUse.ListReturnValue.Count > 0)
                {
                    ExecuteFollowingScripts(0, gameTime, EventToUse.ListReturnValue, out IsCompleted, out Result);
                }
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _ScriptToUsePath = BR.ReadString();

                if (Owner != null)
                {
                    EventToUse = Owner.Copy();
                    EventToUse.Load(_ScriptToUsePath);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_ScriptToUsePath);
            }

            public override AIScript CopyScript()
            {
                return new CustomEvent();
            }

            [Editor(typeof(Selectors.AISelector), typeof(UITypeEditor)),
            CategoryAttribute("Script Attributes"),
            DescriptionAttribute("The AI path"),
            DefaultValueAttribute("")]
            public string ScriptToUsePath
            {
                get
                {
                    return _ScriptToUsePath;
                }
                set
                {
                    _ScriptToUsePath = value;
                }
            }
        }

        public class Return : AIScript, ScriptEvaluator
        {
            public Return()
                : base(100, 50, "Return", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                IsCompleted = true;
                Result = new List<object>(1) { null };
            }

            public override AIScript CopyScript()
            {
                return new Return();
            }
        }

        public class ReturnValue : AIScript, ScriptEvaluator
        {
            public ReturnValue()
                : base(100, 50, "Return Value", new string[0], new string[1] { "Value to Return" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                IsCompleted = true;
                Result = new List<object>(1) { ArrayReferences[0].ReferencedScript.GetContent() };
            }

            public override AIScript CopyScript()
            {
                return new ReturnValue();
            }
        }

        public class ExtractReturnValue : AIScript, ScriptEvaluator
        {
            private List<object> ListReturnValue;
            private int _Index;

            public ExtractReturnValue()
                : base(100, 50, "Extract Return Value", new string[1] { "Value Extracted" }, new string[0])
            {
                ListReturnValue = null;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                if (ListReturnValue == null)
                    ListReturnValue = (List<object>)Input;

                ExecuteFollowingScripts(0, gameTime, ListReturnValue[Index], out IsCompleted, out Result);
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _Index = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_Index);
            }

            public override AIScript CopyScript()
            {
                return new ExtractReturnValue();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public int Index
            {
                get
                {
                    return _Index;
                }
                set
                {
                    _Index = value;
                }
            }
        }

        public class Operator : AIScript, ScriptReference
        {
            private Operators.LogicOperators _LogicOperator;

            public bool CompareValue(Operators.LogicOperators Operator, double Value1, double Value2)
            {
                switch (Operator)
                {
                    case Operators.LogicOperators.Equal:
                        if (Value1 == Value2)
                            return true;
                        else
                            return false;

                    case Operators.LogicOperators.Greater:
                        if (Value1 > Value2)
                            return true;
                        else
                            return false;

                    case Operators.LogicOperators.GreaterOrEqual:
                        if (Value1 >= Value2)
                            return true;
                        else
                            return false;

                    case Operators.LogicOperators.Lower:
                        if (Value1 < Value2)
                            return true;
                        else
                            return false;

                    case Operators.LogicOperators.LowerOrEqual:
                        if (Value1 <= Value2)
                            return true;
                        else
                            return false;

                    case Operators.LogicOperators.NotEqual:
                        if (Value1 != Value2)
                            return true;
                        else
                            return false;
                }

                return false;
            }

            public Operator()
                : base(150, 50, "Logic Operator", new string[0], new string[2] { "Left Number", "Right Number" })
            {
            }

            public object GetContent()
            {
                object Value1 = ArrayReferences[0].ReferencedScript.GetContent();
                object Value2 = ArrayReferences[1].ReferencedScript.GetContent();
                if (Value1 is string && Value2 is string)
                {
                    if (_LogicOperator == Operators.LogicOperators.Equal)
                    {
                        return Value1.ToString() == Value2.ToString();
                    }
                    else if (_LogicOperator == Operators.LogicOperators.NotEqual)
                    {
                        return Value1.ToString() != Value2.ToString();
                    }
                }

                return CompareValue(_LogicOperator, (double)Value1, (double)Value2);
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                LogicOperator = (Operators.LogicOperators)BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write((int)LogicOperator);
            }

            public override AIScript CopyScript()
            {
                return new Operator();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public Operators.LogicOperators LogicOperator
            {
                get
                {
                    return _LogicOperator;
                }
                set
                {
                    _LogicOperator = value;
                }
            }
        }

        public class ForEach : AIScript, ScriptEvaluator
        {
            public ForEach()
                : base(100, 50, "For Each", new string[1] { "On Each Object" }, new string[1] { "Object List" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                bool IsEverythingCompleted = true;
                Result = new List<object>();
                List<object> FollowingScriptResult = new List<object>();

                foreach (object ActiveObject in (List<object>)ArrayReferences[0].ReferencedScript.GetContent())
                {
                    ExecuteFollowingScripts(0, gameTime, ActiveObject, out IsCompleted, out FollowingScriptResult);
                    IsEverythingCompleted &= IsCompleted;
                    Result.AddRange(FollowingScriptResult);
                    if (FollowingScriptResult.Contains("break"))
                    {
                        break;
                    }
                }

                IsCompleted = IsEverythingCompleted;
            }

            public override AIScript CopyScript()
            {
                return new ForEach();
            }
        }

        public class If : AIScript, ScriptEvaluator
        {
            public If()
                : base(120, 50, "If", new string[2] { "Condition is true", "Condition is false" }, new string[1] { "Condition" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                bool IsEverythingCompleted = true;

                if (((bool)ArrayReferences[0].ReferencedScript.GetContent()))
                {
                    ExecuteFollowingScripts(0, gameTime, null, out IsCompleted, out Result);
                    IsEverythingCompleted &= IsCompleted;
                }
                else
                {
                    ExecuteFollowingScripts(1, gameTime, null, out IsCompleted, out Result);
                    IsEverythingCompleted &= IsCompleted;
                }

                IsCompleted = IsEverythingCompleted;
            }

            public override AIScript CopyScript()
            {
                return new If();
            }
        }

        /// <summary>
        /// The variable keep its value through each execution.
        /// </summary>
        public class SetVariable : AIScript, ScriptEvaluator, ScriptReference
        {
            private object VariableValue;
            private string _DefaultValue;

            public SetVariable()
                : base(100, 50, "Set Variable", new string[0], new string[0])
            {
                _DefaultValue = string.Empty;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                VariableValue = Input;
                IsCompleted = true;
                Result = new List<object>();
            }

            public object GetContent()
            {
                return VariableValue;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                double Result;
                _DefaultValue = BR.ReadString();

                if (double.TryParse(DefaultValue, out Result))
                {
                    VariableValue = Result;
                }
                else
                {
                    VariableValue = _DefaultValue;
                }
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_DefaultValue);
            }

            public override AIScript CopyScript()
            {
                return new SetVariable();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public string DefaultValue
            {
                get
                {
                    return _DefaultValue;
                }
                set
                {
                    _DefaultValue = value;
                }
            }
        }

        public class Counter : AIScript, ScriptEvaluator
        {
            private int _StartingValue;
            private int CurrentValue;

            public Counter()
                : base(120, 50, "Counter", new string[] { "Countdown finished" }, new string[] { })
            {
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _StartingValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_StartingValue);
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                --CurrentValue;
                if (CurrentValue <= 0)
                {
                    CurrentValue = _StartingValue;
                    ExecuteFollowingScripts(0, gameTime, null, out IsCompleted, out Result);
                }
                IsCompleted = true;
                Result = new List<object>();
            }

            public override AIScript CopyScript()
            {
                return new Counter();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public int StartingValue
            {
                get
                {
                    return _StartingValue;
                }
                set
                {
                    _StartingValue = value;
                }
            }
        }

        public class GetContent : AIScript, ScriptEvaluator
        {
            public GetContent()
                : base(100, 50, "Get Content", new string[1] { "Received Object" }, new string[1] { "Content" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                ExecuteFollowingScripts(0, gameTime, ArrayReferences[0].ReferencedScript.GetContent(), out IsCompleted, out Result);
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new GetContent();
            }
        }

        public class AddValueToContent : AIScript, ScriptEvaluator
        {
            private double _Value;

            public AddValueToContent()
                : base(150, 50, "Add Value To Content", new string[1] { "Received Object" }, new string[1] { "Content" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                double OriginalValue = (double)ArrayReferences[0].ReferencedScript.GetContent();
                OriginalValue += _Value;
                ExecuteFollowingScripts(0, gameTime, OriginalValue, out IsCompleted, out Result);
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _Value = BR.ReadDouble();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_Value);
            }

            public override AIScript CopyScript()
            {
                return new AddValueToContent();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public double Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }
        }

        public class AddRandomValueToContent : AIScript, ScriptEvaluator
        {
            private double _MinValue;
            private double _MaxValue;

            public AddRandomValueToContent()
                : base(150, 50, "Add Random Value To Content", new string[1] { "Received Object" }, new string[1] { "Content" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                double OriginalValue = (double)ArrayReferences[0].ReferencedScript.GetContent();
                double Difference = _MaxValue - _MinValue;

                OriginalValue = OriginalValue - _MinValue + RandomHelper.Random.NextDouble() * Difference;
                ExecuteFollowingScripts(0, gameTime, OriginalValue, out IsCompleted, out Result);
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _MinValue = BR.ReadDouble();
                _MaxValue = BR.ReadDouble();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_MinValue);
                BW.Write(_MaxValue);
            }

            public override AIScript CopyScript()
            {
                return new AddRandomValueToContent();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public double MinValue
            {
                get
                {
                    return _MinValue;
                }
                set
                {
                    _MinValue = value;
                }
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public double MaxValue
            {
                get
                {
                    return _MaxValue;
                }
                set
                {
                    _MaxValue = value;
                }
            }
        }

        public class GetRandomContentFromList : AIScript, ScriptEvaluator
        {
            public GetRandomContentFromList()
                : base(150, 50, "Get Random Content From List", new string[1] { "Received Object" }, new string[1] { "Content" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                List<object> Target = (List<object>)ArrayReferences[0].ReferencedScript.GetContent();

                ExecuteFollowingScripts(0, gameTime, Target[RandomHelper.Next(Target.Count - 1)], out IsCompleted, out Result);
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new GetRandomContentFromList();
            }
        }

        public class GetFirstContentFromList : AIScript, ScriptReference
        {
            public GetFirstContentFromList()
                : base(150, 50, "Get First Content From List", new string[0], new string[1] { "Content" })
            {
            }

            public object GetContent()
            {
                List<object> Target = (List<object>)ArrayReferences[0].ReferencedScript.GetContent();
                
                return Target[0];
            }

            public override AIScript CopyScript()
            {
                return new GetFirstContentFromList();
            }
        }

        public class GetListCount : AIScript, ScriptReference
        {
            public GetListCount()
                : base(100, 50, "Get List Count", new string[0], new string[1] { "List" })
            {
            }

            public object GetContent()
            {
                List<object> Target = (List<object>)ArrayReferences[0].ReferencedScript.GetContent();
                return (double)Target.Count;
            }

            public override AIScript CopyScript()
            {
                return new GetListCount();
            }
        }
    }
}
