
#define DEBUG // Pour activer la sortie de debug même en mode release


using System;
using System.Diagnostics;






public static class Benchmark
{
	// Variables
	// ---------
	private static Stopwatch stopwatch = new Stopwatch(); 


	// Méthodes
	// --------
	public static void Begin()
	{
		stopwatch.Restart(); 
	}


	public static void End(string description)
	{
		stopwatch.Stop();
    Console.WriteLine($"Execution time: {stopwatch.Elapsed.TotalSeconds:F6}s // {description}");
    Console.WriteLine("");
  }


}
