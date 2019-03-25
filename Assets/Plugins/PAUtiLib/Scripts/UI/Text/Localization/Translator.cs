// Author(s): Paul Calande
// Translator class for translating between languages.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Threading;

public class Translator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("All languages supported by the application.")]
    Language[] languages;
    [SerializeField]
    [Tooltip("The default language to use when no other language is saved.")]
    readonly string defaultLanguage = "English";

    // The name of the currently-selected language.
    string currentLanguage;
    // Whether the current language reads left-to-right or right-to-left.
    bool currentLanguageRTL = false;
    // The node reader for the current translation file.
    JSONNodeReader translationReader;

    private void Awake()
    {
        currentLanguage = PlayerPrefs.GetString("currentLanguage", defaultLanguage);
        Language lang = GetLanguage(currentLanguage);
        if (lang != null)
        {
            SetLanguage(currentLanguage, false);
        }
    }

    // Returns the Language instance that has the given name.
    private Language GetLanguage(string languageName)
    {
        for (int i = 0; i < languages.Length; ++i)
        {
            Language lang = languages[i];
            if (lang.GetName() == languageName)
            {
                return lang;
            }
        }
        // No language with the given name was found, so return null.
        return null;
    }

    private void SetLanguage(Language lang, bool saveChange = true)
    {
        // Update the current language.
        currentLanguage = lang.GetName();
        currentLanguageRTL = lang.IsRightToLeft();
        string tag = lang.GetLanguageTag();

        // Update the thread's culture info for proper string formatting.
        // This affects the formatting of dates, numbers, etc.
        CultureInfo newCultureInfo = new CultureInfo(tag);
        if (newCultureInfo.IsNeutralCulture)
        {
            Debug.LogError("Failed to update CurrentCulture to " + tag
                + " due to it being a neutral culture.");
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = newCultureInfo;
        }

        translationReader = new JSONNodeReader(lang.GetTranslationFile());

        if (saveChange)
        {
            // Save the language choice.
            PlayerPrefs.SetString("currentLanguage", currentLanguage);
        }
    }

    public void SetLanguage(string languageName, bool saveChange = true)
    {
        Language lang = GetLanguage(languageName);
        if (lang == null)
        {
            Debug.LogError("Translator.SetLanguage: Could not find language with the name "
    + languageName);
        }
        else
        {
            SetLanguage(lang, saveChange);
        }
    }

    // Returns true if the current language is supported by the game.
    public bool CurrentLanguageExists()
    {
        return GetLanguage(currentLanguage) != null;
    }

    // Returns true if the current language is right-to-left.
    public bool IsLanguageRTL()
    {
        return currentLanguageRTL;
    }

    // Retrieves ("translates") a string using the current language file.
    // The given key argument is the key to retrieve the corresponding
    // string value from in the language file.
    public string Translate(string translationKey)
    {
        string result;
        if (translationReader.TryGet(translationKey, out result))
        {
            return result;
        }
        else
        {
            string errorStr =
                "ERROR IN Translator.Translate: Couldn't find translation key: "
                + translationKey;
            Debug.LogError(errorStr);
            return errorStr;
        }
    }

    // Retrieves the string at the given key and then formats it.
    public string Format(string translationKey, params object[] args)
    {
        return string.Format(Translate(translationKey), args);
    }

    // Translates a string using the key created by combining the translation key pieces.
    // Each piece is connected with an underscore.
    public string Translate(params string[] translationKeyPieces)
    {
        return Translate(UtilString.Connect("_", translationKeyPieces));
    }
}