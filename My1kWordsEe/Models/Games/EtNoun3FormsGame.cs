public class EtNoun3FormsGame
{
    public EtNoun3FormsGame(
        string nimetavSõna,
        string nimetavLause,
        string omastavSõna,
        string omastavLause,
        string osastavSõna,
        string osastavLause
    )
    {
        this.NimetavSõna = nimetavSõna;
        this.NimetavLause = nimetavLause;
        this.OmastavSõna = omastavSõna;
        this.OmastavLause = omastavLause;
        this.OsastavSõna = osastavSõna;
        this.OsastavLause = osastavLause;
        this.UserNimetavSõna = nimetavSõna;
    }

    public string NimetavSõna { get; private set; }

    public string NimetavLause { get; private set; }

    public string OmastavSõna { get; private set; }

    public string OmastavLause { get; private set; }

    public string OsastavSõna { get; private set; }

    public string OsastavLause { get; private set; }

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

        if (string.Equals(this.NimetavSõna, this.UserNimetavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            this.IsNimetavOk = true;
            Score++;
        }

        if (string.Equals(this.OmastavSõna, this.UserOmastavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            this.IsOmastavOk = true;
            Score++;
        }

        if (string.Equals(this.OsastavSõna, this.UserOsastavSõna.Trim(), StringComparison.OrdinalIgnoreCase))
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
