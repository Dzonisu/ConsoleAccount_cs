using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Security;

namespace ConsoleAccount
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<User> Account = new List<User>();
            bool exit = true;
            do
            {
                Console.WriteLine("\nChoose action: \n1. create account\n2. log in\n3. exit");
                string action = string.Format(Console.ReadLine());
                switch (action)
                {
                    case "1":
                        {
                            Console.Write("\nEnter username: ");
                            string usnm = string.Format(Console.ReadLine());
                            bool usnmIsExist = false;
                            for (int i = 0; i < Account.Count; i++)
                            {
                                if (usnm == Account[i].Username)
                                {
                                    Console.WriteLine("Username already exist!");
                                    usnmIsExist = true;
                                    break;
                                }
                            }
                            if (usnmIsExist) continue;
                            Console.Write("\nEnter your password: ");
                            string pass1 = maskInput();
                            Console.Write("\nRepeat password: ");
                            string pass2 = maskInput();
                            if (pass1 != pass2) { Console.WriteLine("Please try again!"); continue; }

                            string cryptedPassword = Crypt(pass1);
                            string uncrypted = DeCrypt(cryptedPassword);
                            var user = new User(usnm, cryptedPassword, usnmIsExist);
                            if (!usnmIsExist) Account.Add(user);

                            Console.WriteLine($"\nYour username is: {user.Username}");
                            Console.WriteLine($"Crypted password: {cryptedPassword}\nUncrypted Password: {uncrypted}");
                            Console.WriteLine($"{User.numOfUsers} active username(s): ");

                            foreach (User user_name in Account)
                            {
                                Console.WriteLine(user_name);
                            }
                            break;
                        }
                    case "2":
                        {
                            bool tries = false;
                            Console.Write("Enter your username: ");
                            string username = string.Format(Console.ReadLine());
                            bool isExistUsername = false;
                            string actualUsnm = "";
                            int numOfAcc = 0;
                            for (int i = 0; i < Account.Count; i++)
                            {
                                if (username == Account[i].Username)
                                {
                                    isExistUsername = true;
                                    actualUsnm += Account[i].ToString();
                                    numOfAcc = i;

                                }
                            }
                            if (!isExistUsername) { Console.WriteLine("Entered username don't exist!"); continue; }
                            Console.Write("Enter your password: ");
                            string password = maskInput();
                            if (password != DeCrypt(Account[numOfAcc].Password))
                            {
                                Console.WriteLine("\nIncorect password!");
                                continue;
                            }
                            Console.WriteLine($"\n{actualUsnm} is logged in...");
                            Console.WriteLine("Do you want to change password? Y/N");
                            string choice = string.Format(Console.ReadLine());
                            if (!(choice == "Y" || choice == "y"))
                            {
                                break;
                            }

                            int numOfTries = 1;
                            do
                            {
                                numOfTries++;

                                Console.Write("Input old password:");

                                string pass = string.Format(Console.ReadLine());
                                if (pass != DeCrypt(Account[numOfAcc].Password)) { Console.WriteLine("Incorrect input!"); continue; }
                                Console.Write("\nEnter new password: ");
                                string pass1 = maskInput();
                                Console.Write("\nRe-enter new password: ");
                                string pass2 = maskInput();
                                if (pass1 == pass2) Account[numOfAcc].ChangePassword(pass, pass1);
                                else
                                {
                                    Console.WriteLine("\nPasswords are not match!");
                                    continue;
                                }
                                Console.WriteLine($"\nNew password for user {Account[numOfAcc]} is: {DeCrypt(Account[numOfAcc].Password)}");
                                break;

                            } while (tries || numOfTries < 4);
                            break;
                        }
                    case "3":
                        {
                            Console.WriteLine("Do you want to exit? Y/N");
                            string s = string.Format(Console.ReadLine());
                            if (s == "Y" || s == "y") exit = false;
                            break;
                        }

                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }

            } while (exit);
        }
        class User
        {
            public void ChangePassword(string oldPass, string newPass)
            {
                if (oldPass == DeCrypt(this.password))
                {
                    this.password.Remove(0);
                    this.password = Crypt(newPass);
                }
            }
            public static int numOfUsers = 0;
            private string username = "";
            private string password = "";
            public string Username
            {
                get { return username; }
            }
            public string Password
            {
                get { return password; }
            }
            public User(string usnm, string pass, bool isExist)
            {
                if (!isExist)
                {
                    this.username = usnm;
                    numOfUsers++;
                    this.password = pass;
                }
            }
            public override string ToString()
            {
                return string.Format(Username);
            }
        }
        private static string maskInput() //copied from google
        {
            //Console.WriteLine ("Enter your password:");
            string pass = "";
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    pass += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            {
                return pass;
            }
        }
        private static string Crypt(string enteredPassword)
        {
            string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 ~`!@#$%^&*()_-+=:;'\\\"|/?,<.>";
            char[] enPass = enteredPassword.ToCharArray();
            var rnd = new Random();
            var crptPass = new char[enPass.Length * 5];
            int j = 0;
            for (int i = 0; i < crptPass.Length; i++)
            {
                if (i % 5 == 0)
                {
                    crptPass[i] = enPass[j];
                    j++;
                }
                else crptPass[i] = str[rnd.Next(str.Length)];
            }
            var crypted = new String(crptPass);
            return crypted;
        }
        private static string DeCrypt(string crptdPass)
        {
            char[] toArr = crptdPass.ToCharArray();
            char[] pass = new char[crptdPass.Length / 5];
            int j = 0;
            for (int i = 0; i < toArr.Length; i++)
            {
                if (i % 5 == 0)
                {
                    pass[j] = toArr[i];
                    j++;
                }
            }
            string password = new string(pass);
            return password;
        }
    }

}
