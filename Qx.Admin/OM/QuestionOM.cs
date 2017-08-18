﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace Qx.Admin
{
    class QuestionOM : ObjectGrid, IWpfObjectGrid
    {
        public QuestionOM()
        {
            /*toolBar.Items.Add(new Separator());
            var btn = new Button { Content = "שכפל שאלה" };
            btn.Click += new System.Windows.RoutedEventHandler(btn_Click);
            toolBar.Items.Add(btn);*/
        }

        void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var items = GetSelectedItems();
            if (items.Count > 1 || items.Count == 0)
            {
                MessageBox.Show("Pick one question to copy");
                return;
            }
            var q = RemoteObjectProvider.GetQuestionAccess().LoadByName(((LiteQuestion)items[0]).Name);
            new QuestionObjectEdit(q, true).ShowDialog();
        }

        protected override object ItemSource()
        {
            return RemoteObjectProvider.GetLiteQuestionAccess().LoadAll().ToList();
        }

        public void New()
        {
            new QuestionObjectEdit().ShowDialog();
        }

        public void Edit()
        {
            if (GetSelectedItem() == null) return;
            new QuestionObjectEdit(RemoteObjectProvider.GetQuestionAccess().Load((GetSelectedItem() as LiteQuestion).ID)).ShowDialog();
        }

        public void Delete()
        {
            foreach (LiteQuestion question in GetSelectedItems())
            {
                question.IsDeleted = true;
                RemoteObjectProvider.GetLiteQuestionAccess().SaveOrUpdate(question);
            }
            RefreshItemSource();
        }
    }
}
