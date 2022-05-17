using System.Collections.Immutable;
using System.IO.Compression;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

while (true)
{
    real_thread();
}
void real_thread()
{
    int log_number;
    string system_envrionment = Directory.GetCurrentDirectory();
    string new_directory = $"{system_envrionment}\\temporary";
    string data_directory = $"{system_envrionment}\\data";

    if (!Directory.Exists(new_directory))
    {
        Directory.CreateDirectory(new_directory);
    }
    if (!Directory.Exists(data_directory))
    {
        Directory.CreateDirectory(data_directory);
    }
    try
    {
        string read_file = File.ReadAllText("last_log.txt");
        log_number = Int32.Parse(read_file);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        log_number = Constants.file_start;
        File.WriteAllText("last_log.txt", log_number.ToString());
        Directory.Delete(new_directory, true);

    }
    int return_result = real_threaded(log_number);
    File.WriteAllText("last_log.txt", return_result.ToString());






    return;
}


int real_threaded(int log_number)
{


   

    List<Task> thread_wrangler = new List<Task>();
    List<int> new_int = new List<int>();
    HttpClient client = new HttpClient();


    for (int i = 0; i < 600; i++)
    {
        new_int.Add(log_number + i);

    }
    foreach (int i in new_int)
    {
        thread_wrangler.Add(Task.Run(() => web_scraper(i, client)));


    }
    Task.WaitAll(thread_wrangler.ToArray());
    return log_number + 600;




}
async Task web_scraper(int log_number, HttpClient client)
{
    try
    {
        string interop_string = $"{Constants.website}{log_number}{Constants.website_end}";

        HttpResponseMessage response = await client.GetAsync(interop_string);


        string system_environment = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        System.Net.Http.HttpContent content = response.Content;
        var contentStream = await content.ReadAsStreamAsync();

        string create_file = $"{log_number}_delete.zip";
        var stream_read = File.Create($"{system_environment}\\{create_file}");

        await contentStream.CopyToAsync(stream_read);
        contentStream.Close();
        stream_read.Close();
        file_stream(system_environment, log_number, create_file);

    }
    catch (Exception e)
    {
        Console.WriteLine("\nException Caught!");
        Console.WriteLine("Message :{0} ", e.Message);

    }


}

int file_stream(string system_environment, int log_number, string create_file)
{
    string current_directory = Directory.GetCurrentDirectory();
    string extract_to = $"{current_directory}\\temporary\\{log_number}";

    string file_path = $"{system_environment}\\{create_file}";
    string good_rename = $"{extract_to}\\log_{log_number}.txt";
    if (!Directory.Exists(extract_to))
    {
        Directory.CreateDirectory(extract_to);
        ZipFile.ExtractToDirectory(file_path, extract_to, true);
        string open_text = $"{extract_to}\\log_{log_number}.log";
        File.Move(open_text, good_rename);
        File.Delete(open_text);
        File.Delete(file_path);
    }
    else
    {
        File.Delete(file_path);
    }
    StreamReader fs = File.OpenText(good_rename);
    List<string> append_deez = new List<string>();
    int line_number = 0;
    while (!fs.EndOfStream)
    {
        try
        {
            string parse_this = fs.ReadLine();
            parse_this = parse_this.ToLower();
            string append = parser_function(parse_this);
            append_deez.Add(append);
            line_number++;

        }
        catch (Exception e)
        {
            fs.Close();
            string data_path = $"{system_environment}\\data\\{log_number}_data";
            Console.WriteLine(e.Message);
            if (!Directory.Exists(data_path))
            {
                Directory.CreateDirectory(data_path);
            }
            File.WriteAllLines($"{data_path}\\{log_number}.txt", append_deez);
            return 0;


        }
    }
    string data_path_2 = $"{current_directory}\\data\\{log_number}_data";
    if (!Directory.Exists(data_path_2))
    {
        Directory.CreateDirectory(data_path_2);
    }
    File.WriteAllLines($"{data_path_2}\\{log_number}.txt", append_deez);
    fs.Close();
    return 1;

}
string parser_function(string parse)
{
    ImmutableArray<string> const_teams = ImmutableArray.Create(new string[] { "<red>", "<blue>" });


    ImmutableArray<string> const_values = ImmutableArray.Create(new string[] { "triggered", "killed", "picked up item", "commited suicide with", "changed role to", "spawned as" });
    ImmutableArray<string> merc_values = ImmutableArray.Create(new string[] { "scout", "soldier", "pyro", "demo", "heavyweapons", "engineer", "medic", "sniper", "spy" });
    ImmutableArray<string> result_of_action = ImmutableArray.Create(new string[] { @"""damage""", @"""healed""", @"""medkit_small""", @"""medkit_medium""", @"""medkit_large""", @"""player_builtobject""", @"""killedobject""", @"""chargedeployed""", @"""pointcaptured""", @"""player_carryobject""", @"""player_dropobject""", @"""shot_fired""", @"""medic_death""" });
    ImmutableArray<string> team_talk = ImmutableArray.Create(new string[] { "say_team" });
    ImmutableArray<string> against = ImmutableArray.Create(new string[] { "against" });
    ImmutableArray<string> start_times = ImmutableArray.Create(new string[] { "round_start", "round_setup_begin", "round_setup_end", "round_win", "round_length", " current score ", "round_overtime" });
    ImmutableArray<string> read_values = ImmutableArray.Create(new string[] { "damage", "weapon", "healing", "winner", "round" });
    ImmutableArray<string> action_word = ImmutableArray.Create(new string[] { "with" });

    string dictionaryString = null;

    ImmutableArray<ImmutableArray<string>> const_arrays = ImmutableArray.Create(new ImmutableArray<string>[] { const_values, merc_values, result_of_action, team_talk, against, start_times, read_values, action_word });

    List<int> player_number = new List<int>();

    int new_x = 0;

    Dictionary<int, int> people_involved = new Dictionary<int, int>();
    while (new_x < 50)
    {
        string interp_string = $"<{new_x}>";
        if (parse.Contains(interp_string))
        {
            int length_of_search = parse.Length;

            int new_index = parse.IndexOf(interp_string, 0, length_of_search);

            int end_this = parse.IndexOf(@"""", new_index, length_of_search - new_index);

            foreach (string loop_string in const_teams)
            {


                int check = parse.IndexOf(@"""", new_index, length_of_search - new_index);


                int good_check = parse.IndexOf(loop_string, new_index, length_of_search - new_index);

                int new_check = check - new_index;

                if (new_index < end_this && check != -1 && (check - 8) < good_check)
                {
                    int team_index = const_teams.IndexOf(loop_string);
                    people_involved.Add(new_x, team_index);


                    break;

                }


            }

            new_x++;
        }
        else
        {
            new_x++;
        }


    }

    if (people_involved.Count > 0)
    {
        string new_string = $"{people_involved.ElementAt(0).Key}:{people_involved.ElementAt(0).Value},";
        dictionaryString = dictionaryString + new_string;
    }

    int length_of_arr = const_arrays.Length;
    for (int x = 0; x < length_of_arr; x++)
    {
        foreach (string search_this in const_arrays[x])
        {
            bool test = parse.Contains(search_this); // Handling actions
            int search_to = parse.Length;
            int final_append;
            if (test)
            {
                int new_good_index = const_arrays[x].IndexOf(search_this);
                switch (x)
                {
                    case 0:
                        final_append = new_good_index + Constants.action_values;
                        string new_append = $"       {final_append}    ";
                        dictionaryString += new_append;
                        break;
                    case 1:
                        final_append = new_good_index + Constants.merc_values;
                        string new_append_2 = $"       {final_append}   ";
                        dictionaryString += new_append_2;
                        break;
                    case 2:
                        final_append = new_good_index + Constants.item_values;
                        string new_append_3 = $"       {final_append}   ";
                        dictionaryString += new_append_3;
                        break;

                    case 3:
                        int search_index = parse.IndexOf(search_this, 0, search_to);
                        string sub_string = parse.Substring(search_index);

                        final_append = new_good_index;
                        string new_append_4 = $"    {search_index.ToString()}   {sub_string}";
                        dictionaryString += new_append_4;

                        break;
                    case 4:

                        final_append = new_good_index - 1;
                        string who_did_it = $"{people_involved.ElementAt(1).Key}:{people_involved.ElementAt(1).Value},";
                        string new_append_5 = $"       {final_append}           {who_did_it} ";
                        dictionaryString += new_append_5;
                        break;
                    case 5:
                        int search_terms = parse.IndexOf(search_this, 0, search_to);
                        int test_this = parse.IndexOf(@"""", search_terms, search_to - search_terms);
                        int test_this_3 = parse.IndexOf(@"""", 0, search_to);
                        int length_2 = search_this.Length+search_terms;

                        
                        if (test_this_3!=search_terms-1 || test_this!=length_2 )
                        {
                            break;
                        }
                        else
                        {
                            string sub_string_2 = parse.Substring(search_terms, search_this.Length);
                            string new_append_6 = $"    {search_this}   {sub_string_2}";
                            dictionaryString += new_append_6;
                            break;
                        }

                    case 6:
                        int search_terms_2 = parse.IndexOf(search_this, 0, search_to);
                        int no_quotes = parse.IndexOf(@"""", search_terms_2, search_to - search_terms_2);

                        int test_this_2 = parse.IndexOf(@")", search_terms_2, search_to - search_terms_2);

                        int search_this_length = search_this.Length;
                        int there_should_be_a_QUOTE = parse.IndexOf(@"""", search_terms_2, search_to - search_terms_2);
                        int comparison_this = there_should_be_a_QUOTE - search_this_length;


                        if (test_this_2 == -1 || no_quotes == search_terms_2 + 1 || comparison_this != search_terms_2 + 1)
                        {
                            break;
                        }
                        else
                        {
                            int length = test_this_2 - search_terms_2;

                            string sub_string_2 = parse.Substring(search_terms_2, length);


                            string new_append_6 = $"   {sub_string_2}   ";

                            dictionaryString += new_append_6;
                        }
                        break;
                    case 7:
                        int search_terms_3 = parse.IndexOf(search_this, 0, search_to);

                        int first_index = parse.IndexOf(@"""", search_terms_3, search_to - search_terms_3);
                        int last_index = parse.IndexOf(@"""", first_index, search_to - first_index); // fix this
                        string sub_string_3 = parse.Substring(first_index, last_index - first_index);
                        string final_append_3 = $" 0   {sub_string_3}";
                        dictionaryString += final_append_3;
                        break;



                }



            }


        }
    }


    return dictionaryString;
}




struct team
{
    public bool team_id;
    public List<int> player_id;
    public Player player_data;




}

struct Player
{
    public int id;
    public List<string> classes;
    public List<string> actions;
    public List<string> values;



}

static class Constants
{
    public const int file_start = 1000000;
    public const string website = "https://logs.tf/logs/log_";
    public const int file_end = 3189484;
    public const string website_end = ".log.zip";
    public const int merc_values = 100;
    public const int action_values = 900;
    public const int item_values = 3000;



}