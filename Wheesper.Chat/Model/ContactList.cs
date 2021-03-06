﻿using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;

namespace Wheesper.Chat.Model
{
    public class ContactList :BindableBase
    {
        public ContactList(string listname)
        {
            Groupname = listname;

            Contacts = new ListCollectionView(contacts);
            Contacts.CurrentChanged += contactSelectedItemChanged;
        }

        public string Groupname
        {
            get { return groupname; }
            set { groupname = value; }
        }
        public ListCollectionView Contacts { get; private set; }

        private string groupname = null;
        private ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

        public void Add(Contact contact)
        {
            contacts.Add(contact);
        }
        public void Remove(Contact contact)
        {
            contacts.Remove(contact);
        }

        private void contactSelectedItemChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(DateTime.Now.ToString());
            Contact currentContact = Contacts.CurrentItem as Contact;
            if (currentContact == null)
                return;
            Debug.WriteLine("current select contact:");
            // TODO: 
            Debug.WriteLine(currentContact.EMail);
        }
    }
}
