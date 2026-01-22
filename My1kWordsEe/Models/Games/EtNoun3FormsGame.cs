using System.ComponentModel;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Semantics;

// todo: make game object a class
// todo: move struct part to the factory
// struct is requred by serializer
public struct EtNoun3FormsGame
{
    public EtNoun3FormsGame() { }

    [Description("Sõna ainsuse nimetavas käändes")]
    public string NimetavSõna { get; set; }

    [Description("Lihtne lause, kus sõna on ainsuse nimetavas käändes")]
    public string NimetavLause { get; set; }

    [Description("Sõna ainsuse omastavas käändes")]
    public string OmastavSõna { get; set; }

    [Description("Lihtne lause, kus sõna on ainsuse omastavas käändes")]
    public string OmastavLause { get; set; }

    [Description("Sõna ainsuse osastavas käändes")]
    public string OsastavSõna { get; set; }

    [Description("Lihtne lause, kus sõna on ainsuse osastavas käändes")]
    public string OsastavLause { get; set; }

    // remove?
    [JsonIgnore]
    public string UserNimetavLause { get; set; }

    // remove?
    [JsonIgnore]
    public string UserOmastavLause { get; set; }

    // remove?
    [JsonIgnore]
    public string UserOsastavLause { get; set; }

    [JsonIgnore]
    public string UserNimetavSõna { get; set; }

    [JsonIgnore]
    public string UserOmastavSõna { get; set; }

    [JsonIgnore]
    public string UserOsastavSõna { get; set; }

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

        if (string.Equals(this.NimetavSõna, this.UserNimetavSõna, StringComparison.OrdinalIgnoreCase))
        {
            this.IsNimetavOk = true;
            Score++;
        }

        if (string.Equals(this.OmastavSõna, this.UserOmastavSõna, StringComparison.OrdinalIgnoreCase))
        {
            this.IsOmastavOk = true;
            Score++;
        }

        if (string.Equals(this.OsastavSõna, this.UserOsastavSõna, StringComparison.OrdinalIgnoreCase))
        {
            this.IsOsastavOk = true;
            Score++;
        }

        this.IsFinished = true;
    }
}