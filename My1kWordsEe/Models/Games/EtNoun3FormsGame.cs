using My1kWordsEe.Models;

public class EtNoun3FormsGame
{
    public EtNoun3FormsGame(
        TranslatedString nimetavSõna,
        TranslatedString nimetavLause,
        TranslatedString omastavSõna,
        TranslatedString omastavLause,
        TranslatedString osastavSõna,
        TranslatedString osastavLause
    )
    {
        this.NimetavSõna = nimetavSõna;
        this.NimetavLause = nimetavLause;
        this.OmastavSõna = omastavSõna;
        this.OmastavLause = omastavLause;
        this.OsastavSõna = osastavSõna;
        this.OsastavLause = osastavLause;
        // preset default case because user knows it
        this.UserNimetavSõna = nimetavSõna.Et;
    }

    public TranslatedString NimetavSõna { get; private set; }

    public TranslatedString NimetavLause { get; private set; }

    public TranslatedString OmastavSõna { get; private set; }

    public TranslatedString OmastavLause { get; private set; }

    public TranslatedString OsastavSõna { get; private set; }

    public TranslatedString OsastavLause { get; private set; }

    public string UserNimetavSõna { get; set; } = string.Empty;

    public string UserOmastavSõna { get; set; } = string.Empty;

    public string UserOsastavSõna { get; set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public int Score { get; private set; } = 0;

    public bool IsNimetavOk { get; private set; } = false;

    public bool IsOmastavOk { get; private set; } = false;

    public bool IsOsastavOk { get; private set; } = false;

    public void Submit()
    {
        if (this.IsFinished)
        {
            return;
        }

        if (string.Equals(this.NimetavSõna.Et, this.UserNimetavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            this.IsNimetavOk = true;
            Score++;
        }

        if (string.Equals(this.OmastavSõna.Et, this.UserOmastavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            this.IsOmastavOk = true;
            Score++;
        }

        if (string.Equals(this.OsastavSõna.Et, this.UserOsastavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            this.IsOsastavOk = true;
            Score++;
        }

        this.IsFinished = true;
    }

    public void GiveUp()
    {
        this.IsFinished = true;
    }
}
