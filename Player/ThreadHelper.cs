using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Player
{
    // http://stackoverflow.com/questions/10775367/cross-thread-operation-not-valid-control-textbox1-accessed-from-a-thread-othe

    public static class ThreadHelper
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        delegate void AddListViewItemCallback(Form f, ListView ctrl, ListViewItem item);
        delegate void ClearListViewCallback(Form f, ListView ctrl);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text;
            }
        }

        public static void AddListViewItem(Form form, ListView ctrl, ListViewItem item)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                AddListViewItemCallback d = new AddListViewItemCallback(AddListViewItem);
                form.Invoke(d, new object[] { form, ctrl, item });
            }
            else
            {
                ctrl.Items.Add(item);
            }
        }

        public static void ClearListView(Form form, ListView ctrl)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                ClearListViewCallback d = new ClearListViewCallback(ClearListView);
                form.Invoke(d, new object[] { form, ctrl });
            }
            else
            {
                ctrl.Items.Clear();
            }
        }
    }
}
