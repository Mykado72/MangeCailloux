using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

/// <summary>
/// Classe qui regroupe toutes les fonctions utilies présentent dans le jeu.
/// </summary>
public static class OurUtilitysFonctions {
    public static bool useJaggedTab = false;
    const string stringforSalt = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want

    /// <summary>
    /// Convertis un caractère en entier d'après la correspondance : '0' => 0 'A' => 10 'B' => 11 etc...
    /// </summary>
    /// <param name="hexChar">Le caractère à transformer en entier</param>
    /// <returns>Une int correspondante au char. Retourne 0 si le caractère n'est pas prie en charge.</returns>
    public static int CharToInt(char hexChar) {
        switch (hexChar) {
            case '0': return 0;
            case '1': return 1;
            case '2': return 2;
            case '3': return 3;
            case '4': return 4;
            case '5': return 5;  // caisse marron
            case '6': return 6;  // caisse rouge
            case '7': return 7;  // caisse orange
            case '8': return 8;  // caisse Verte
            case '9': return 9;  // caisse Bleue
            case 'A': return 10; // bouton marron
            case 'B': return 11; // bouton bleu
            case 'C': return 12; // bouton orange
            case 'D': return 13; // bouton rouge
            case 'E': return 14; // bouton vert
            case 'F': return 15; 
            case 'G': return 16;
            case 'H': return 17;
            case 'I': return 18;
            case 'J': return 19;
            case 'K': return 20;
            case 'L': return 21;
            case 'M': return 22;
            case 'N': return 23;
            case 'O': return 24;
            case 'P': return 25;
            case 'Q': return 26;
            case 'R': return 27;
            case 'S': return 28;
            case 'T': return 29;
            case 'U': return 30;
            case 'V': return 31;
            case 'W': return 32;
            case 'X': return 33;
            case 'Y': return 34;
            case 'Z': return 35;
            case 'a': return 36; // Caisse marron sur inter marron.
            case 'b': return 37; // Caisse marron sur inter bleu.
            case 'c': return 38; // Caisse marron sur inter jaune.
            case 'd': return 39; // Caisse marron sur inter rouge.
            case 'e': return 40; // Caisse marron sur inter vert.
            case 'f': return 41; // Caisse rouge sur inter marron.
            case 'g': return 42;
            case 'h': return 43;
            case 'i': return 44;
            case 'j': return 45;
            case 'k': return 46; // Caisse orange sur inter marron.
            case 'l': return 47;
            case 'm': return 48;
            case 'n': return 49;
            case 'o': return 50;
            case 'p': return 51; // Caisse verte sur inter marron.
            case 'q': return 52;
            case 'r': return 53;
            case 's': return 54;
            case 't': return 55;
            case 'u': return 56; // Caisse bleue sur inter marron.
            case 'v': return 57;
            case 'w': return 58;
            case 'x': return 59;
            case 'y': return 60;
            case 'z': return 61; // Inutilisé, mais à éviter car au millieux de la série des caisses sur interrupteurs.
            //    z pourrais devenir Caisse Y sur inter Jaune
            case '%': return 62; // Caisse Y sur inter marron.
            case '&': return 63; // Caisse Y sur inter bleu.
            case '#': return 64; // Caisse Y sur inter orange.
            case '$': return 65; // Caisse Y sur inter rouge.
            case '!': return 66; // Caisse Y sur inter vert.
            case '(': return 67; // Arbuste1
            case ')': return 68; // Arbuste2
            case '=': return 69; // Rouage
            case '{': return 70; // ResteCrane
            case '}': return 71; // ResteCotes
            case '[': return 72; // GroundStone3
            case '+': return 73; // +Mur
            case 'à': return 74; // à Mur
            case 'ä': return 75; // ä Mur
            case 'â': return 76; // â Mur

                //  '5' caisse marron | '6' caisse rouge | '7' caisse orange | '8' caisse Verte | '9' caisse Bleue |  caisse Jaune
                //  'A' bouton marron | 'D' bouton rouge | 'C' bouton orange | 'E' bouton vert  | 'B' bouton bleu  | 
                //  'a' marron marron | 'f' c roug b mar | 'k' orange marron | 'p' verte - marr | 'u' bleue marron | '%' jaune marron  
                //  'b' marron rouge  | 'i' rouge rouge  | 'n' orange rouge  | 's' verte rouge  | 'x' bleue rouge  | '$' jaune rouge 
                //  'c' marron orange | 'h' rouge orange | 'm' orange orange | 'r' verte orange | 'w" bleue orange | '#' jaune orange
                //  'd' marron vert   | 'j' rouge vert   | 'o' orange vert   | 't' verte vert   | 'y' bleue vert   | '!' jaune vert
                //  'e' marron bleu   | 'g' rouge bleu   | 'l' orange bleu   | 'q' verte bleu   | 'v' bleue bleu   | '&" jaune bleu

        }
        return 0;
    }

    /// <summary>
    /// Non utilisée pour le moment
    /// Permet de savoir si l'élement est un mur.
    /// Utile dans l'éditeur pour ne pas changer les bordures qui sont des murs lors d'un changment de taille de grille.
    /// </summary>
    /// <param name="hexChar">Le caractère à tester</param>
    /// <returns>Vrai si c'est un mur, sinon faux.</returns>
    public static bool CharIsAWall(char hexChar) {
        switch (hexChar) {
            case '1':
            case 'X':
            case 'I':
            case 'H':
            case 'J':
            case '+':
            case 'à':
            case 'â':
            case 'ä':
                return true;
            default:
                return false;

        }
    }

    public static Vector3 RoundIntVector(Vector3 value, float scale = 2f)
    {
        value.x = Mathf.RoundToInt(value.x / scale);
        value.y = Mathf.RoundToInt(value.y / scale);
        value.z = Mathf.RoundToInt(value.z / scale);

        return value;
    }

    public static Vector3 ClampPosition(Vector3 positionToClamp)
    {
        positionToClamp.x = Mathf.Clamp(positionToClamp.x, 0, Movements.tabLevel3d.GetLength(0) - 1);
        positionToClamp.y = Mathf.Clamp(positionToClamp.y, 0, Movements.tabLevel3d.GetLength(1) - 1);
        positionToClamp.z = Mathf.Clamp(positionToClamp.z, 0, Movements.tabLevel3d.GetLength(2) - 1);
        return positionToClamp;
    }

    public static string HashMD5(string txt)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] bArray = Encoding.ASCII.GetBytes(txt);
            {
                return Encoding.ASCII.GetString(md5.ComputeHash(bArray));
            }
        }
    }

    public static string EncryptText(string input, string password) {
        // Get the bytes of the string
        byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        // Hash the password with SHA256
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

        string result = Convert.ToBase64String(bytesEncrypted);

        return result;
    }

    public static string DecryptText(string input, string password) {
        // Get the bytes of the string
        byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

        string result = Encoding.UTF8.GetString(bytesDecrypted);

        return result;
    }

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
        byte[] encryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream()) {
            using (RijndaelManaged AES = new RijndaelManaged()) {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }

        return encryptedBytes;
    }

    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
        byte[] decryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream()) {
            using (RijndaelManaged AES = new RijndaelManaged()) {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }

    public static string RandomSalt(int m_length)
    {
        string myString=null;
        m_length = UnityEngine.Random.Range(0, stringforSalt.Length); 

        for (int i = 0; i < m_length; i++)
        {
            myString += stringforSalt[UnityEngine.Random.Range(0, stringforSalt.Length)];
        }
        return myString;
    }

    public static string[] GetResourcesDirectories()
    {
        List<string> result = new List<string>();
        Stack<string> stack = new Stack<string>();
        // Add the root directory to the stack
        stack.Push(Application.dataPath);
        // While we have directories to process...
        while (stack.Count > 0)
        {
            // Grab a directory off the stack
            string currentDir = stack.Pop();
            try
            {
                foreach (string dir in Directory.GetDirectories(currentDir))
                {
                    if (Path.GetFileName(dir).Equals("Resources"))
                    {
                        // If one of the found directories is a Resources dir, add it to the result
                        result.Add(dir);
                    }
                    // Add directories at the current level into the stack
                    stack.Push(dir);
                }
            }
            catch
            {
                Debug.LogError("Directory " + currentDir + " couldn't be read from.");
            }
        }
        return result.ToArray();
    }



    public static int InterFromCaisseSurInter(int obectID) {
        switch (obectID) {
            case 36:
            case 41:
            case 46:
            case 51:
            case 56:
            case 62:
                return 10;// bouton marron

            case 40:
            case 42:
            case 47:
            case 52:
            case 57:
            case 63:
                return 11;// bouton bleu

            case 38:
            case 43:
            case 48:
            case 53:
            case 58:
            case 64:
                return 12;// bouton orange

            case 37:
            case 44:
            case 49:
            case 54:
            case 59:
            case 65:
                return 13;// bouton rouge

            case 39:
            case 45:
            case 50:
            case 55:
            case 60:
            case 66:
                return 14;// bouton vert


        }
        return 10; // Par défaut on donne un interrupteur marron. Mais le code n'est jamais censé arriver ici.


    }
    public static int CaisseFromCaisseSurInter(int obectID) {
        switch (obectID) {
            case 36:
            case 37:
            case 38:
            case 39:
            case 40:
                return 5; // caisse marron
            case 41:
            case 42:
            case 43:
            case 44:
            case 45:
                return 6; // caisse rouge
            case 46:
            case 47:
            case 48:
            case 49:
            case 50:
                return 7; // caisse orange
            case 51:
            case 52:
            case 53:
            case 54:
            case 55:
                return 8; // caisse verte
            case 56:
            case 57:
            case 58:
            case 59:
            case 60:
                return 9; // caisse bleue
            case 62:
            case 63:
            case 64:
            case 65:
            case 66:
                return 34; // caisse sans couleur

        }
        return 5; // Par défaut on donne une caisse marron. Mais le code n'est jamais censé arriver ici.
    }

}
