using System.ComponentModel;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Semantics;

public class EtNoun3FormsGame
{
    // Nimetav, // (nominatiiv)
    // Omastav, // (genitiiv)
    // Osastav, // (partitiiv)

    [Description("todo")]
    public string NimetavSõna { get; set; }

    [Description("todo")]
    public string NimetavLause { get; set; }

    [Description("todo")]
    public string OmastavSõna { get; set; }

    [Description("todo")]
    public string OmastavLause { get; set; }

    [Description("todo")]
    public string OsastavSõna { get; set; }

    [Description("todo")]
    public string OsastavLause { get; set; }

    [JsonIgnore]
    public string UserNimetavLause { get; set; }

    [JsonIgnore]
    public string UserOmastavLause { get; set; }

    [JsonIgnore]
    public string UserOsastavLause { get; set; }

    [JsonIgnore]
    public bool IsFinished { get; private set; } = false;

    [JsonIgnore]
    public int Score { get; private set; } = 0;

    [JsonIgnore]
    public bool IsNimetavOk { get; private set; } = false;

    [JsonIgnore]
    public bool IsOmastavOk { get; private set; } = false;

    [JsonIgnore]
    public bool IsOsastavOk { get; private set; } = false;

    public void Submit()
    {
        if (this.IsFinished)
        {
            return;
        }


        if (string.Equals(this.NimetavLause, this.UserNimetavLause, StringComparison.OrdinalIgnoreCase))
        {
            this.IsNimetavOk = true;
            Score++;
        }

        if (string.Equals(this.OmastavLause, this.UserOmastavLause, StringComparison.OrdinalIgnoreCase))
        {
            this.IsOmastavOk = true;
            Score++;
        }

        if (string.Equals(this.OsastavLause, this.UserOsastavLause, StringComparison.OrdinalIgnoreCase))
        {
            this.IsOsastavOk = true;
            Score++;
        }

        this.IsFinished = true;
    }
}