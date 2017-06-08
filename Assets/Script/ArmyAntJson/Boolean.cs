using System;



	public class JBoolean : IUnit
    {
        public static IUnit isThis(string text)
        {
            var realText = text.Trim().Trim(new char[] { '\r', '\n' });
            switch (realText)
            {
                case "true":
                    return new JBoolean(true);
                case "false":
                    return new JBoolean(false);
                default:
                    return null;
            }
        }

        public JBoolean(bool v = false)
		{
			value = v;
		}

		public string String
		{
			get
			{
				return value.ToString().ToLower();
			}
			set
			{
				switch (value.Trim().Trim(new char[] { '\r', '\n' }))
				{
					case "true":
						this.value = true;
						break;
					case "false":
					default:
						this.value = false;
						break;
				}
			}
		}

		public ArmyAntJson.EType Type
		{
			get
			{
				return ArmyAntJson.EType.Boolean;
			}
		}

        private bool value = false;

        public bool ToBool()
        {
            return value;
        }

        public int ToInt()
        {
            return value ? 1 : 0;
        }

        public double ToFloat()
        {
            return value ? 1.0 : 0.0;
        }

        public JObject ToObject()
        {
            return null;
        }

        public JArray ToArray()
        {
            return null;
        }

        public override string ToString()
        {
            return null;
        }
    }


