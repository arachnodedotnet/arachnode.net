#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Arachnode.Proxy.Authentication
{
    ///<summary>Stores a dictionary with username/password combinations.</summary>
    ///<remarks>This class can be used by a SOCKS5 listener.</remarks>
    ///<remarks>This class uses an MD5 has to store the passwords in a secure manner.</remarks>
    ///<remarks>The username is treated in a case-insensitive manner, the password is treated case-sensitive.</remarks>
    public class AuthenticationList
    {
        /// <summary>Holds the value of the Listing property.</summary>
        private readonly StringDictionary m_Listing = new StringDictionary();

        ///<summary>Gets the StringDictionary that's used to store the user/pass combinations.</summary>
        ///<value>A StringDictionary object that's used to store the user/pass combinations.</value>
        protected StringDictionary Listing
        {
            get { return m_Listing; }
        }

        ///<summary>Gets an array with all the keys in the authentication list.</summary>
        ///<value>An array of strings containing all the keys in the authentication list.</value>
        public string[] Keys
        {
            get
            {
                ICollection keys = Listing.Keys;
                string[] ret = new string[keys.Count];
                keys.CopyTo(ret, 0);
                return ret;
            }
        }

        ///<summary>Gets an array with all the hashes in the authentication list.</summary>
        ///<value>An array of strings containing all the hashes in the authentication list.</value>
        public string[] Hashes
        {
            get
            {
                ICollection values = Listing.Values;
                string[] ret = new string[values.Count];
                values.CopyTo(ret, 0);
                return ret;
            }
        }

        ///<summary>Adds an item to the list.</summary>
        ///<param name="Username">The username to add.</param>
        ///<param name="Password">The corresponding password to add.</param>
        ///<exception cref="ArgumentNullException">Either Username or Password is null.</exception>
        public void AddItem(string Username, string Password)
        {
            if (Password == null)
            {
                throw new ArgumentNullException();
            }
            AddHash(Username, Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Password))));
        }

        ///<summary>Adds an item to the list.</summary>
        ///<param name="Username">The username to add.</param>
        ///<param name="PassHash">The hashed password to add.</param>
        ///<exception cref="ArgumentNullException">Either Username or Password is null.</exception>
        public void AddHash(string Username, string PassHash)
        {
            if (Username == null || PassHash == null)
            {
                throw new ArgumentNullException();
            }
            if (Listing.ContainsKey(Username))
            {
                Listing[Username] = PassHash;
            }
            else
            {
                Listing.Add(Username, PassHash);
            }
        }

        ///<summary>Removes an item from the list.</summary>
        ///<param name="Username">The username to remove.</param>
        ///<exception cref="ArgumentNullException">Username is null.</exception>
        public void RemoveItem(string Username)
        {
            if (Username == null)
            {
                throw new ArgumentNullException();
            }
            Listing.Remove(Username);
        }

        ///<summary>Checks whether a user/pass combination is present in the collection or not.</summary>
        ///<param name="Username">The username to search for.</param>
        ///<param name="Password">The corresponding password to search for.</param>
        ///<returns>True when the user/pass combination is present in the collection, false otherwise.</returns>
        public bool IsItemPresent(string Username, string Password)
        {
            return IsHashPresent(Username, Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Password))));
        }

        ///<summary>Checks whether a username is present in the collection or not.</summary>
        ///<param name="Username">The username to search for.</param>
        ///<returns>True when the username is present in the collection, false otherwise.</returns>
        public bool IsUserPresent(string Username)
        {
            return Listing.ContainsKey(Username);
        }

        ///<summary>Checks whether a user/passhash combination is present in the collection or not.</summary>
        ///<param name="Username">The username to search for.</param>
        ///<param name="PassHash">The corresponding password hash to search for.</param>
        ///<returns>True when the user/passhash combination is present in the collection, false otherwise.</returns>
        public bool IsHashPresent(string Username, string PassHash)
        {
            return Listing.ContainsKey(Username) && Listing[Username].Equals(PassHash);
        }

        ///<summary>Clears the authentication list.</summary>
        public void Clear()
        {
            Listing.Clear();
        }

        // private variables
    }
}