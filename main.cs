using System;
using System.Text;
using System.Collections.Generic;

enum Options
{
    ROCK,
    PAPER,
    SCISSORS
}


struct Turn
{
    public Turn(Options option)
    {
        Option = option;
    }

    public Options Option
    {
        get;
        private set;
    }

    public override string ToString()
    {
        return Option.ToString();
    }
}

interface PlayerTurnStrategy
{
    Turn MakeTurn();
}

class CLIStrategy : PlayerTurnStrategy
{
    private string options_string;
    public CLIStrategy()
    {
        options_string = string.Join(", ", Enum.GetNames<Options>());
    }
    public Turn MakeTurn()
    {
        while (true)
        {
            Console.Write($"Введите ход {options_string}: ");
            string tn = Console.ReadLine().Trim();
            try
            {
                return new Turn(Enum.Parse<Options>(tn));
            }
            catch (Exception)
            {
                Console.WriteLine("Неправильный ход");
            }
        }
    }
}

class RandomStrategy : PlayerTurnStrategy
{
    private Random r;
    Options[] values;
    public RandomStrategy()
    {
        r = new Random();
        values = Enum.GetValues<Options>();
    }
    public Turn MakeTurn()
    {
        return new Turn((Options)values[r.Next(values.Length)]);
    }
}

class MirrorStrategy : PlayerTurnStrategy
{
    private Player opponent;
    private IDictionary<string, Turn> situation;

    public MirrorStrategy(Player opponent, IDictionary<string, Turn> situation)
    {
        this.opponent = opponent;
        this.situation = situation;
    }
    public Turn MakeTurn()
    {
        if (this.situation.ContainsKey(this.opponent.Name))
        {
            return this.situation[this.opponent.Name];
        }
        else
        {
            Random r = new Random();
            Options[] values = Enum.GetValues<Options>();
            return new Turn((Options)values[r.Next(values.Length)]);
        }
    }
}

class Player
{
    public Player(string name, PlayerTurnStrategy pts)
    {
        Name = name;
        TurnStrategy = pts;
    }

    protected PlayerTurnStrategy TurnStrategy { get; set; }
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        protected set
        {
            if (value.Length == 0)
            {
                throw new Exception("Empty Player Name");
            }
            this._name = value;
        }
    }

    public Turn MakeTurn()
    {
        return TurnStrategy.MakeTurn();
    }
    public virtual string InitName()
    {
        return this._name;
    }
    public void Account(IDictionary<string, Turn> situation)
    {
        Console.WriteLine($"{this.Name}: {situation[this.Name]}");
    }
}

class CLIPlayer : Player
{
    public CLIPlayer() : base("CLIPlayer", new CLIStrategy())
    {
    }
    public override string InitName()
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Enter new name for player {this.Name}");

                this.Name = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
            break;
        }
        return this.Name;
    }
}

class RandomBotPlayer : Player
{
    public RandomBotPlayer() : base("RandomBot", new RandomStrategy())
    {

    }
    public override string InitName()
    {
        StringBuilder sb = new StringBuilder();
        Random r = new Random();
        sb.Append("RandomBot");
        sb.Append(r.Next(10).ToString());
        sb.Append(r.Next(10).ToString());
        sb.Append(r.Next(10).ToString());
        this.Name = sb.ToString();
        return this.Name;
    }
}

class MirrorBotPlayer : Player
{
    public MirrorBotPlayer(Player opponent, IDictionary<string,Turn> situation):
        base("MirrorBot", new MirrorStrategy(opponent, situation))
    {

    }
    public override string InitName()
    {
        StringBuilder sb = new StringBuilder();
        Random r = new Random();
        sb.Append("MirrorBot");
        sb.Append(r.Next(10).ToString());
        sb.Append(r.Next(10).ToString());
        sb.Append(r.Next(10).ToString());
        this.Name = sb.ToString();
        return this.Name;
    }
}

class Program
{
    public static void Main(string[] args)
    {
        var situation = new Dictionary<string, Turn>();
        var pl = new CLIPlayer();
        var bot = new MirrorBotPlayer(pl,situation);
        pl.InitName();
        Console.WriteLine($"Имя Бота: {bot.InitName()}");
        while (true)
        {
            situation[pl.Name] = pl.MakeTurn();
            situation[bot.Name] = bot.MakeTurn();
            pl.Account(situation);
            bot.Account(situation);
        }
    }
}