﻿//
// AccessibleBackend.cs
//
// Author:
//       Vsevolod Kukol <sevoku@microsoft.com>
//
// Copyright (c) 2017 Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Xwt.Accessibility;
using Xwt.Backends;

namespace Xwt.GtkBackend
{
	public class AccessibleBackend : IAccessibleBackend
	{
		Gtk.Widget widget;
		IAccessibleEventSink eventSink;
		ApplicationContext context;

		public AccessibleBackend ()
		{
		}

		public void Initialize (IAccessibleEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void Initialize (IWidgetBackend parentWidget, IAccessibleEventSink eventSink)
		{
			var backend = parentWidget as WidgetBackend;
			Initialize (backend?.Widget, eventSink);
		}

		public void Initialize (object parentWidget, IAccessibleEventSink eventSink)
		{
			this.eventSink = eventSink;
			widget = parentWidget as Gtk.Widget;
			if (widget == null)
				throw new ArgumentException ("The widget is not a Gtk.Widget.", nameof (parentWidget));
		}

		public void InitializeBackend (object frontend, ApplicationContext context)
		{
			this.context = context;
		}

		public Rectangle Bounds {
			get {
				int x = 0, y = 0, w = 0, h = 0;
				(widget.Accessible as Atk.Component)?.GetExtents (out x, out y, out w, out h, Atk.CoordType.Screen);
				return new Rectangle (x, y, w, h);
			}
			set {
				(widget.Accessible as Atk.Component)?.SetExtents ((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height, Atk.CoordType.Screen);
			}
		}

		public string Description {
			get {
				return widget.Accessible.Description;
			}
			set {
				widget.Accessible.Description = value;
			}
		}

		public string Label { get; set; }

		public Role Role {
			get {
				return widget.Accessible.Role.ToXwtRole ();
			}

			set {
				widget.Accessible.Role = value.ToAtkRole ();
			}
		}

		public string RoleDescription { get; set; }

		public string Title { get; set; }

		public string Value {
			get {
				if (widget.Accessible is Atk.Value) {
					GLib.Value val = GLib.Value.Empty;
					(widget.Accessible as Atk.Value)?.GetCurrentValue (ref val);
					return val.Val.ToString ();
				}
				if (widget.Accessible is Atk.Text) {
					var atkText = (widget.Accessible as Atk.Text);
					return atkText?.GetText (0, atkText.CharacterCount - 1);
				}
				return null;
			}
			set {
				if (widget.Accessible is Atk.Value) {
					GLib.Value val = GLib.Value.Empty;
					(widget.Accessible as Atk.Value)?.SetCurrentValue (new GLib.Value (value));
				} else if (widget.Accessible is Atk.EditableText) {
					var atkText = (widget.Accessible as Atk.EditableText);
					atkText.TextContents = value;
				}
			}
		}

		public bool IsAccessible {
			get {
				return widget.Accessible?.Role != Atk.Role.Invalid;
			}
			set {
				// TODO
				//if (widget.Accessible != null)
				//	return widget.Accessible.Role = Atk.Role.Invalid;
			}
		}

		public void DisableEvent (object eventId)
		{
			throw new NotImplementedException ();
		}

		public void EnableEvent (object eventId)
		{
			throw new NotImplementedException ();
		}
	}
}
