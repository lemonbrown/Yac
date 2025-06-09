using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScraper.Helpers {

    public static class NFLTeamConverter {
        public static string GetTeamAbbreviation(string fullTeamName) {
            // Remove common prefixes and clean up the input
            string cleanName = fullTeamName?.Trim();

            // Handle cases where input might include city name
            if (cleanName == null)
                return string.Empty;

            // Extract just the team name (last word typically, but handle special cases)
            return cleanName switch {
                // AFC East
                "Buffalo Bills" or "Bills" => "BUF",
                "Miami Dolphins" or "Dolphins" => "MIA",
                "New England Patriots" or "Patriots" => "NE",
                "New York Jets" or "Jets" => "NYJ",

                // AFC North
                "Baltimore Ravens" or "Ravens" => "BAL",
                "Cincinnati Bengals" or "Bengals" => "CIN",
                "Cleveland Browns" or "Browns" => "CLE",
                "Pittsburgh Steelers" or "Steelers" => "PIT",

                // AFC South
                "Houston Texans" or "Texans" => "HOU",
                "Indianapolis Colts" or "Colts" => "IND",
                "Jacksonville Jaguars" or "Jaguars" => "JAX",
                "Tennessee Titans" or "Titans" => "TEN",

                // AFC West
                "Denver Broncos" or "Broncos" => "DEN",
                "Kansas City Chiefs" or "Chiefs" => "KC",
                "Las Vegas Raiders" or "Raiders" or "Oakland Raiders" => "LV",
                "Los Angeles Chargers" or "Chargers" or "San Diego Chargers" => "LAC",

                // NFC East
                "Dallas Cowboys" or "Cowboys" => "DAL",
                "New York Giants" or "Giants" => "NYG",
                "Philadelphia Eagles" or "Eagles" => "PHI",
                "Washington Commanders" or "Commanders" or "Washington Redskins" or "Redskins" or "Washington Football Team" => "WAS",

                // NFC North
                "Chicago Bears" or "Bears" => "CHI",
                "Detroit Lions" or "Lions" => "DET",
                "Green Bay Packers" or "Packers" => "GB",
                "Minnesota Vikings" or "Vikings" => "MIN",

                // NFC South
                "Atlanta Falcons" or "Falcons" => "ATL",
                "Carolina Panthers" or "Panthers" => "CAR",
                "New Orleans Saints" or "Saints" => "NO",
                "Tampa Bay Buccaneers" or "Buccaneers" => "TB",

                // NFC West
                "Arizona Cardinals" or "Cardinals" => "ARI",
                "Los Angeles Rams" or "Rams" or "St. Louis Rams" => "LAR",
                "San Francisco 49ers" or "49ers" => "SF",
                "Seattle Seahawks" or "Seahawks" => "SEA",

                // Default case
                _ => ExtractFromTeamName(cleanName)
            };
        }

        // Fallback method to try to extract abbreviation from team name
        private static string ExtractFromTeamName(string teamName) {
            if (string.IsNullOrEmpty(teamName))
                return string.Empty;

            // Try to match partial team names or city names
            if (teamName.Contains("Bill"))
                return "BUF";
            if (teamName.Contains("Dolphin"))
                return "MIA";
            if (teamName.Contains("Patriot"))
                return "NE";
            if (teamName.Contains("Jet"))
                return "NYJ";
            if (teamName.Contains("Raven"))
                return "BAL";
            if (teamName.Contains("Bengal"))
                return "CIN";
            if (teamName.Contains("Brown"))
                return "CLE";
            if (teamName.Contains("Steeler"))
                return "PIT";
            if (teamName.Contains("Texan"))
                return "HOU";
            if (teamName.Contains("Colt"))
                return "IND";
            if (teamName.Contains("Jaguar"))
                return "JAX";
            if (teamName.Contains("Titan"))
                return "TEN";
            if (teamName.Contains("Bronco"))
                return "DEN";
            if (teamName.Contains("Chief"))
                return "KC";
            if (teamName.Contains("Raider"))
                return "LV";
            if (teamName.Contains("Charger"))
                return "LAC";
            if (teamName.Contains("Cowboy"))
                return "DAL";
            if (teamName.Contains("Giant"))
                return "NYG";
            if (teamName.Contains("Eagle"))
                return "PHI";
            if (teamName.Contains("Commander") || teamName.Contains("Redskin"))
                return "WAS";
            if (teamName.Contains("Bear"))
                return "CHI";
            if (teamName.Contains("Lion"))
                return "DET";
            if (teamName.Contains("Packer"))
                return "GB";
            if (teamName.Contains("Viking"))
                return "MIN";
            if (teamName.Contains("Falcon"))
                return "ATL";
            if (teamName.Contains("Panther"))
                return "CAR";
            if (teamName.Contains("Saint"))
                return "NO";
            if (teamName.Contains("Buccaneer"))
                return "TB";
            if (teamName.Contains("Cardinal"))
                return "ARI";
            if (teamName.Contains("Ram"))
                return "LAR";
            if (teamName.Contains("49er") || teamName.Contains("Forty"))
                return "SF";
            if (teamName.Contains("Seahawk"))
                return "SEA";

            // City name matching
            if (teamName.Contains("Buffalo"))
                return "BUF";
            if (teamName.Contains("Miami"))
                return "MIA";
            if (teamName.Contains("New England"))
                return "NE";
            if (teamName.Contains("Baltimore"))
                return "BAL";
            if (teamName.Contains("Cincinnati"))
                return "CIN";
            if (teamName.Contains("Cleveland"))
                return "CLE";
            if (teamName.Contains("Pittsburgh"))
                return "PIT";
            if (teamName.Contains("Houston"))
                return "HOU";
            if (teamName.Contains("Indianapolis"))
                return "IND";
            if (teamName.Contains("Jacksonville"))
                return "JAX";
            if (teamName.Contains("Tennessee"))
                return "TEN";
            if (teamName.Contains("Denver"))
                return "DEN";
            if (teamName.Contains("Kansas City"))
                return "KC";
            if (teamName.Contains("Las Vegas") || teamName.Contains("Oakland"))
                return "LV";
            if (teamName.Contains("Dallas"))
                return "DAL";
            if (teamName.Contains("Philadelphia"))
                return "PHI";
            if (teamName.Contains("Washington"))
                return "WAS";
            if (teamName.Contains("Chicago"))
                return "CHI";
            if (teamName.Contains("Detroit"))
                return "DET";
            if (teamName.Contains("Green Bay"))
                return "GB";
            if (teamName.Contains("Minnesota"))
                return "MIN";
            if (teamName.Contains("Atlanta"))
                return "ATL";
            if (teamName.Contains("Carolina"))
                return "CAR";
            if (teamName.Contains("New Orleans"))
                return "NO";
            if (teamName.Contains("Tampa Bay"))
                return "TB";
            if (teamName.Contains("Arizona"))
                return "ARI";
            if (teamName.Contains("Los Angeles") && teamName.Contains("Ram"))
                return "LAR";
            if (teamName.Contains("Los Angeles") && teamName.Contains("Charger"))
                return "LAC";
            if (teamName.Contains("San Francisco"))
                return "SF";
            if (teamName.Contains("Seattle"))
                return "SEA";

            return ExtractFromUniqueAbbrevation(teamName); // Return empty string if no match found
        }

        private static string ExtractFromUniqueAbbrevation(string abbv) {

            if (abbv == "KAN")
                return "KC";

            return abbv;
        }
    }
}
