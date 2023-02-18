using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    public enum Colors { green, yellow, purple, red, blue, orange, none };
    public List<Colors> colorSequence;
    public int level;
    public int highScore;
    public float secondsRemaining;
    public bool timerActive;

    // Scene components
    [SerializeField] Button btnGreen;
    [SerializeField] Button btnYellow;
    [SerializeField] Button btnPurple;
    [SerializeField] Button btnRed;
    [SerializeField] Button btnBlue;
    [SerializeField] Button btnOrange;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI highScoreText;



    // Start is called before the first frame update
    void Start()
    {
        colorSequence = new List<Colors>();

        level = 1;
        highScore = 0;
        secondsRemaining = 20.0f;
        timerActive = false;
    }



    // Update is called once per frame
    void Update()
    {
        // Visually update the displayed level, high score, and remaining time for the current round.
        levelText.text = "Level: " + level;
        highScoreText.text = "High Score: " + highScore;
        timerText.text = "Timer: " + (int)secondsRemaining;

        // If theres time left, tick it down, if not the game is lost
        if (timerActive)
        {
            if (secondsRemaining > 0)
                secondsRemaining -= Time.deltaTime;
            else
                loseGame(); 
        }
    }



    // Generate and display a random sequence of colors at the start of a round
    public void startRound()
    {
        secondsRemaining = 20;
        generateSequence();
        highlightSequence();
    }



    // Generate a sequence of random colors for the current level
    public void generateSequence()
    {
        // Generate random numbers and cast them to a Colors enum to create a random sequence of colors
        for (int i = 0; i < (2 + level); i++)
            colorSequence.Add((Colors)Random.Range(0, 6));
    }



    // Highlights each button in the sequence in order 
    void highlightSequence()
    {
        int loopCount = 0;

        // Iterate through the sequence of colors and highlight each respective button in order
        // NOTE: Due to coroutines not halting the program's execution, each coroutine is given
        //       a delay to ensure that each action is properly offset from one another.
        for (int i = colorSequence.Count - 1; i >= 0; i--)
        {
            // Set the selected color
            if (colorSequence[i] == Colors.green)
                StartCoroutine(highlightSquare(btnGreen, loopCount * 0.8f));
            else if (colorSequence[i] == Colors.yellow)
                StartCoroutine(highlightSquare(btnYellow, loopCount * 0.8f));
            else if (colorSequence[i] == Colors.purple)
                StartCoroutine(highlightSquare(btnPurple, loopCount * 0.8f));
            else if (colorSequence[i] == Colors.red)
                StartCoroutine(highlightSquare(btnRed, loopCount * 0.8f));
            else if (colorSequence[i] == Colors.blue)
                StartCoroutine(highlightSquare(btnBlue, loopCount * 0.8f));
            else if (colorSequence[i] == Colors.orange)
                StartCoroutine(highlightSquare(btnOrange, loopCount * 0.8f));

            loopCount++;
        }

        // Activate the timer once all buttons have flashed
        StartCoroutine(activateTimer(loopCount * 0.8f));
    }



    // Highlights the passed in button after the specified amnount of time
    IEnumerator highlightSquare(Button button, float delay)
    {
        // Remember the button's orginal color
        Color orginalColor = button.GetComponent<Image>().color;
        
        // Wait specified amount of time to ensure each button highlight is offset from one another
        yield return new WaitForSeconds(delay);

        // The button flashes black then back to it's orginal color
        button.GetComponent<Image>().color = Color.black;
        yield return new WaitForSeconds(0.4f);
        button.GetComponent<Image>().color = orginalColor;
        yield return new WaitForSeconds(0.2f);
    }



    // Check if the color selected is the next in the sequence and act accordingly
    public void colorSelected(string color)
    {
        // Immediently return to ignore any button presses done prior to the timer starting
        if (!timerActive)
            return;

        Colors selectedColor = Colors.none;

        // Set the selected color
        if (color == "green")
            selectedColor = Colors.green;
        else if (color == "yellow")
            selectedColor = Colors.yellow;
        else if (color == "purple")
            selectedColor = Colors.purple;
        else if (color == "red")
            selectedColor = Colors.red;
        else if (color == "blue")
            selectedColor = Colors.blue;
        else
            selectedColor = Colors.orange;
        
        // If the selected color is correct, then remove it from the sequence. 
        // If the incorrect color is chosen the game is lost.
        if (colorSequence[colorSequence.Count - 1] == selectedColor)
            colorSequence.RemoveAt(colorSequence.Count - 1);
        else
            loseGame();  

        // Complete the level if the full sequence of buttons were pressed and the game wasn't just lost
        if (colorSequence.Count <= 0 && timerActive)
            completeLevel();
    }



    // Increment the to the next level and reset some values to set up the next level
    void completeLevel()
    {
        level++;
        secondsRemaining = 20.0f;
        timerActive = false;

        // Update highscore if needed
        if (level > highScore)
            highScore = level;

        // All buttons flash Green to indicate you passed
        StartCoroutine(flashColorAllButtons(Color.green));
    }



    // Reset the game values including the level when the user loses
    void loseGame()
    {
        level = 1;
        timerActive = false;
        secondsRemaining = 20.0f;
        colorSequence.Clear();

        // All buttons flash Red to indicate you failed
        StartCoroutine(flashColorAllButtons(Color.red));
    }



    // Activates the timer after the specified delay so it occurs after each button is highlighted
    IEnumerator activateTimer(float delay)
    {
        // Wait specified amount of time to ensure that the timer begins ticking after all the buttons have been highlighted
        yield return new WaitForSeconds(delay);

        // Activate timer for the round
        timerActive = true;
    }



    // Flashes each colored button with the specified color
    IEnumerator flashColorAllButtons(Color flashColor)
    {
        Color[] orginalColors = new Color[6];

        // Save the orginal colors of each button
        orginalColors[0] = btnGreen.GetComponent<Image>().color;
        orginalColors[1] = btnYellow.GetComponent<Image>().color;
        orginalColors[2] = btnPurple.GetComponent<Image>().color;
        orginalColors[3] = btnRed.GetComponent<Image>().color;
        orginalColors[4] = btnBlue.GetComponent<Image>().color;
        orginalColors[5] = btnOrange.GetComponent<Image>().color;

        // All buttons turn to the specified color 
        btnGreen.GetComponent<Image>().color = flashColor;
        btnYellow.GetComponent<Image>().color = flashColor;
        btnPurple.GetComponent<Image>().color = flashColor;
        btnRed.GetComponent<Image>().color = flashColor;
        btnBlue.GetComponent<Image>().color = flashColor;
        btnOrange.GetComponent<Image>().color = flashColor;   

        yield return new WaitForSeconds(0.7f);

        // Return all buttons to their orginal colors
        btnGreen.GetComponent<Image>().color = orginalColors[0];
        btnYellow.GetComponent<Image>().color = orginalColors[1];
        btnPurple.GetComponent<Image>().color = orginalColors[2];
        btnRed.GetComponent<Image>().color = orginalColors[3];
        btnBlue.GetComponent<Image>().color = orginalColors[4];
        btnOrange.GetComponent<Image>().color = orginalColors[5]; 
    }
}