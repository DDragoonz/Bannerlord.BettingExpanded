using TaleWorlds.Library;

namespace BettingExpanded.UI
{
    public class BettingSimpleTextVM : ViewModel
    {

        public BettingSimpleTextVM(string text)
        {
            _text = text;
            _color = Color.White;
        }

        public BettingSimpleTextVM(string text, Color color)
        {
            _text = text;
            _color = color;
        }
        
        [DataSourceProperty]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                bool flag = value != _text;
                if (flag)
                {
                    _text = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }
        
        [DataSourceProperty]
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                bool flag = value != _color;
                if (flag)
                {
                    _color = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        private string _text;
        private Color _color;
    }
}