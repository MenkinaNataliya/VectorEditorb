/*
	Copyright c 2003 by RiskCare Ltd.  All rights reserved.

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions
	are met:
	1. Redistributions of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.
	2. Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
	IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
	ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
	FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
	DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
	OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
	HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
	LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
	OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	SUCH DAMAGE.
*/


using System.Drawing;
using System.Collections;
using System;
using System.Drawing.Drawing2D;


namespace SvgNet.SvgTypes
{

	/// <summary>
	/// Represents a CSS2 style, as applied to an SVG element.
	/// </summary>
	public class SvgStyle : ICloneable
	{
		Hashtable _styles = new Hashtable();

		public SvgStyle()
		{
		}

		public SvgStyle(string s)
		{
			FromString(s);
		}

		/// <summary>
		/// Creates a new style, but does not do a deep copy on the members in the style.  Thus if any of these are
		/// not strings, they meay be left referred to by more than one style or element.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			SvgStyle r = new SvgStyle();
			r += this;
			return r;
		}

		/// <summary>
		/// Creates a style from a GDI+ pen object.  Most properties of the pen are implemented, but GDI+ allows fine control over line-capping which 
		/// has no equivalent in SVG.
		/// </summary>
		/// <param name="pen"></param>
		public SvgStyle(Pen pen)
		{
			SvgColor strokeCol = new SvgColor(((SolidBrush)pen.Brush).Color);
			Set("stroke", strokeCol);
			Set("stroke-width", pen.Width);
			Set("fill", "none");

			switch(pen.EndCap)
			{
				case LineCap.Round:
					Set("stroke-linecap", "round");break;
				case LineCap.Square:
					Set("stroke-linecap", "square");break;
				case LineCap.Flat:
					Set("stroke-linecap", "butt");break;
			}

			switch(pen.LineJoin)
			{
				case LineJoin.Bevel:
					Set("stroke-linejoin", "bevel");break;
				case LineJoin.Miter:
					Set("stroke-linejoin", "miter");break;
				case LineJoin.Round:
					Set("stroke-linejoin", "round");break;
			}

			//converting between adobe and ms miter limits is very hard because adobe have never explained what the value means.
			Set("stroke-miterlimit", pen.MiterLimit/2 + 4f);

			float[] dashes = null;

			switch(pen.DashStyle)
			{
				case DashStyle.Dash:
					dashes = new float[]{3,1}; break;
				case DashStyle.DashDot:
					dashes = new float[]{3,1,1,1}; break;
				case DashStyle.DashDotDot:
					dashes = new float[]{3,1,1,1,1}; break;
				case DashStyle.Dot:
					dashes = new float[]{1,1}; break;
				case DashStyle.Custom:
					dashes = pen.DashPattern; break;
			}

			if (dashes != null)
			{
				//MS GDI changes dash pattern to match width of line; svg does not.
				for(int i=0; i < dashes.Length; ++i)
				{
					dashes[i] *= pen.Width;
				}

				Set("stroke-dasharray", new SvgNumList(dashes));
			}

			Set("opacity", pen.Color.A/255f);
			
		}

		/// <summary>
		/// Creates a style based on a GDI brush object.  Only works for solid brushes; pattern brushes are not yet emulated.
		/// </summary>
		/// <param name="brush"></param>
		public SvgStyle(SolidBrush brush)
		{
			SvgColor col = new SvgColor(((SolidBrush)brush).Color);
			Set("fill", col);
			Set("stroke", "none");
			Set("opacity", ((SolidBrush)brush).Color.A/255f);
		}

		/// <summary>
		/// Creates a style based on a GDI+ font object.  GDI+ allows many subtle specifications which have no SVG equivalent.
		/// </summary>
		/// <param name="font"></param>
		public SvgStyle(Font font)
		{
			Set("font-family", font.FontFamily.Name);

			if (font.Bold)
				Set("font-weight", "bolder");

			if (font.Italic)
				Set("font-style", "italic");

			if (font.Underline)
				Set("text-decoration", "underline");
			
			Set("font-size", font.SizeInPoints.ToString() + "pt");

		}

		/// <summary>
		/// Sets a style.  The key must be a string but the value can be anything (e.g. SvgColor).  If and when the element that owns this style is written out
		/// to XML, <c>ToString</c> will be called on the value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void Set(string key, object val)
		{
			if (val == null || val.ToString() == "")
			{
				_styles.Remove(key);
				return;
			}

			_styles[key] = val;
		}

		/// <summary>
		/// Gets the value for a given key.
		/// </summary>
		public object Get(string key)
		{
			return _styles[key];
		}

		/// <summary>
		/// Parses a CSS string representation as used in SVG.
		/// </summary>
		/// <param name="s"></param>
		public void FromString(string s)
		{
			try
			{
				string[] pairs = s.Split(';');

				foreach(string pair in pairs)
				{
					string[] kv = pair.Split(':');
					if (kv.Length == 2)
						Set(kv[0].Trim(), kv[1].Trim());
				}
			}
			catch(Exception)
			{
				throw new SvgException("Invalid style string", s);
			}
		}

		/// <summary>
		/// Outputs a CSS string representation as used in SVG.
		/// </summary>
		public override string ToString()
		{
			string val;
			string result = "";

			foreach(string s in _styles.Keys)
			{
				val = _styles[s].ToString();
				result += s;
				result += ":";
				result += val;
				result += ";";
			}

			return result;
		}

		/// <summary>
		/// A basic way to enumerate the styles.
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return _styles.Keys;
			}
		}

		/// <summary>
		/// A quick way to get and set style elements.
		/// </summary>
		public object this[string attname]
		{
			get
			{
				return _styles[attname];
			}
			set
			{
				_styles[attname] = value;
			}
		}


		public static implicit operator SvgStyle(string s)
		{
			return new SvgStyle(s);
		}

		/// <summary>
		/// Adds two SvgStyles together, resulting in a new object that contains all the attributes of both styles.
		/// Attributes are copied deeply, i.e. cloned if they are <c>ICloneable</c>.
		/// </summary>
		public static SvgStyle operator + (SvgStyle lhs, SvgStyle rhs)
		{
			SvgStyle res = new SvgStyle();

			foreach(string key in lhs._styles.Keys)
			{
				object o = lhs[key];
				if (typeof(ICloneable).IsInstanceOfType(o))
					res[key] = ((ICloneable)o).Clone();
				else
					res[key] = o;
			}

			foreach(string key in rhs._styles.Keys)
			{
				object o = rhs[key];
				if (typeof(ICloneable).IsInstanceOfType(o))
					res[key] = ((ICloneable)o).Clone();
				else
					res[key] = o;
			}

			return res;
		}
	}
}