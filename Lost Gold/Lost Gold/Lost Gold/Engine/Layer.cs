using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lost_Gold.Engine
{
    public class Layer
    {
        protected string _name;
        protected int _width;
        protected int _height;

        public Layer(string name, int width, int height)
        {
            _name = name;
            _width = width;
            _height = height;
        }
    }
}
