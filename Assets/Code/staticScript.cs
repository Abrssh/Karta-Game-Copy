using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public static class staticScript
{
	public static int playerBid=1;

	public static int roundCounter=1;

	public static int audioCounter = 0;
	public static int[] audioNum = { 0, 1, 2, 3, 4};// used for shuffle
	public static bool audioNumShuffled = false; // used to identify if the audioNum has been shuffled or not
	public static bool audioStart = false; // used to identify if there is a music when the game starts

	public static float[] round1= { 0, 0, 0, 0 };
	public static float[] round2= { 0, 0, 0, 0 };
	public static float[] round3= { 0, 0, 0, 0 };
	public static float[] round4= { 0, 0, 0, 0 };
	public static float[] round5= { 0, 0, 0, 0 };

}
