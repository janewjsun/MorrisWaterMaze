using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Output.cs is a synchronous output function for writing data to files
 * Output.cs should be active within the first scene of your task but doesn't require being active in other scenes
 * If you would like different output files for different tasks, you need to create more static path variable and run the validation/format functions on them
 */
public class Output : MonoBehaviour
{
    //Editor Variables for File(s)
    [Header("Output")]
    public bool generateOutput; //do we want to create a file
    public bool pathData; //do we want to output the path
    public static bool outputting, continuous; //accessible variables for generateOutput and pathData
    public string inputName;    // what the file name is
    public static string excelBuffer, excelPathBuffer;
    //Editor Variables for Tracking :=> Could be expanded to multiple tracked objects

    //Scripting Variables for file(s) and tracking
    private string fileName; //name given in editor
    private string  excelPath,  excelHeader, excelInfo; //names of the .txt and .xls file paths & what their headers and information lines should contain
    private string excelPathPath; //names of the .txt and .xls file paths & what their headers and information lines should contain
    public static string accessibleExcelPath, accessibleExcelPathPath;
    // Start is called before the first frame update
    void Start()
    {
        //initialize accessible variables
        outputting = generateOutput;
        continuous = pathData;

        Debug.Log("This is where we error check and validate our file names");
        //if the researcher wants output to be generated but both file types are unselected
        if (generateOutput)
        {        
            //path validation
            ValidatePath();
            //formatting of the file
            FormatFile();
        }

        //initialize empty buffer(s)
        excelBuffer = "";

        if (pathData)
        {
            excelPathBuffer = "";
        }

    }//End of Start

    //ValidatePath is called once in Start()
    //ValidatePath is used to make sure that the file(s) given are valid (i.e. they dont already exist)
    private void ValidatePath()
    {
        //set the file names to be equal to the given string (we don't directly check the given string for variable security reasons; i.e. we don't want to accidentally override it)
        // fileName = inputName + "_" + SceneManager.GetActiveScene().name; //can change SceneManager...name if using multi-scene setup
        fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        //Debug.Log("Our file names are: " + fileNames);
        //if the fileNames is NOT an empty string
        if (fileName != "")
        {
                //then we set the path to be our current directory and give the log.xls extension (NOTE: this is an older extension that excel may claim to be "corrupted." It is not and the newer extension doesn'w work properly with line breaks).
                excelPath = Application.dataPath + "/logs/" + fileName + ".xls";
                Debug.Log("Our excelPath is: " + excelPath);
                accessibleExcelPath = excelPath;

                //and if the file doesnt exist
                if (!File.Exists(excelPath))
                {
                    using (StreamWriter swe = File.CreateText(excelPath))
                    {
                        swe.WriteLine(fileName + "_Output_File");
                    }
                    //then we write to the file its name + Output File on the first line
                    // File.WriteAllText(excelPath, fileNames + " Output File \n");
                }
                //however, if the file exists
                else
                {
                    //then we will throw an error letting them know, and stop the experiment before it progresses
                    ///uncomment this for build and remove below two lines -> throw new System.Exception("One or more of the file names already exists");
                    Debug.Log("One or more of the file names already exists");
                    UnityEditor.EditorApplication.isPlaying = false;
                }

            if (pathData)
            {
                excelPathPath = Application.dataPath + "/" + fileName + "_path.xls";
                //and if the file doesnt exist
                accessibleExcelPathPath = excelPathPath;
                if (!File.Exists(excelPathPath))
                {
                    using (StreamWriter swe = File.CreateText(excelPathPath))
                    {
                        swe.WriteLine(fileName + "_Output_File");
                    }
                    //then we write to the file its name + Output File on the first line
                    // File.WriteAllText(excelPath, fileNames + " Output File \n");
                }
                //however, if the file exists
                else
                {
                    //then we will throw an error letting them know, and stop the experiment before it progresses
                    ///uncomment this for build and remove below two lines -> throw new System.Exception("One or more of the file names already exists");
                    Debug.Log("One or more of the path file names already exists");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
        }
        //however, if the file name is an empty string
        else
        {
            //then we will throw an error letting them know, and stop the experiment before it progresses
            ///uncomment this for build and remove below two lines -> throw new System.Exception("Please give an output file name!");
            Debug.Log("Please give an output file name!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }//End of ValidatePath

    //FormatFile is called once in Start()
    //FormatFile is used to setup the columns in the ouputfile
    private void FormatFile()
    {
            //write the columns: Time, Trial, TargetName, and TargetNumber to the xls file
            //File.AppendAllText(excelPath, "Time \tTrial \tTargetName \tTargetNumber");
            // excelHeader += "Block \tTrial \tTrialTime(ms) \tTargetName \tTargetNumber \tTargX \tTargZ \tResponseX \tResponseZ \tResponseFromTarget(m)";
            excelHeader += "Block \tTrial \tTime (s) \tDistance (units) \tEfficiency (ideal/actual)";
            using (StreamWriter swe = File.AppendText(excelPath))
            {
                swe.WriteLine(excelHeader);
            }

            if (pathData)
            {
                //write the columns: HeadX, HeadZ, and HeadAngle to the xls file
                //File.AppendAllText(excelPath, "\tHeadX \tHeadZ \tHeadAngle");
                excelHeader += "\tHeadX \tHeadZ \tHeadAngle";
                using (StreamWriter swe = File.AppendText(excelPathPath))
                {
                    swe.WriteLine(excelHeader);
                }
        }

    }//End of FormatFile
    /*

How to incorporate into your own .cs file(s):

Continuous path data example:
    Update()
    ...
        //check if we are outputting and if we want the path
        if (Output.outputting && Output.continuous)
            {
                    //"Block \tTrial \tTrialTime(ms) \tTargetName \tTargetNumber \tTargX \tTargZ \tResponseX \tResponseZ \tResponseFromTarget(m) \tHeadX \tHeadZ \tHeadRot";
                    
                    //add position data to a buffer variable every frame for continuous tracking (split for readability)

                    Output.excelPathBuffer += string.Format("{0} \t{1} \t{2}", navigationTrialType.ToString(), this.parentTask.repeatCount, Time.time * 1000);
                    Output.excelPathBuffer += string.Format("\t{0} \t{1} \t{2} \t{3} \t{4} \t{5} \t{6} \t{7} \t{8} \t{9}\n",
                        current.name, current.transform.GetSiblingIndex(), current.transform.position.x,
                        current.transform.position.z, "NULL",
                        "NULL", "NULL", navigator.transform.position.x, navigator.transform.position.z, Camera.main.transform.eulerAngles.y);
                }
            }
    //NOTE: We will write to the file at the end of the trial (see below)
    ...
    //End of Update

Trial-by-trial output example:
    Update()
    ...

    if(endOfTrial)//this should be your conditional check for moving on
    
    //check if we are generating output files
    if (Output.outputting)
        {
            //add information to a buffer variable (note your info may be different)
            Output.excelBuffer += string.Format("{0} \t{1} \t{2}", navigationTrialType.ToString(), this.parentTask.repeatCount, navTime * 1000);
            
            //file writing code
            using (StreamWriter swe = File.AppendText(Output.accessibleExcelPath))
            {
                //conditionals to add varying degrees of information
                if (navigationTrialType == NavType.Training)
                {
                    Output.excelBuffer += string.Format("\t{0} \t{1} \t{2} \t{3}", current.name, current.transform.GetSiblingIndex(), current.transform.position.x, current.transform.position.z);

                }
                else if (navigationTrialType == NavType.Recall)
                {
                    Output.excelBuffer += string.Format("\t{0} \t{1} \t{2} \t{3} \t{4} \t{5} \t{6}", current.name, current.transform.GetSiblingIndex(), current.transform.position.x, current.transform.position.z, navigator.transform.position.x, navigator.transform.position.z, Vector2.Distance(new Vector2(current.transform.position.x, current.transform.position.z), new Vector2(navigator.transform.position.x, navigator.transform.position.z)));
                }
                
                //writing the buffer to the file
                swe.WriteLine(Output.excelBuffer);
            }
            //clear the buffer
            Output.excelBuffer = "";

            //if we are tracking path data
            if (Output.continuous)
            {

                //"Block \tTrial \tTrialTime(ms) \tTargetName \tTargetNumber \tTargX \tTargZ \tResponseX \tResponseZ \tResponseFromTarget(m) \tHeadX \tHeadZ \tHeadRot";
                
                //add the last line of information
                Output.excelPathBuffer += string.Format("{0} \t{1} \t{2}", navigationTrialType.ToString(), this.parentTask.repeatCount, Time.time * 1000);

                if (navigationTrialType == NavType.Exploration)
                {
                    Output.excelPathBuffer += string.Format("\t{0} \t{1} \t{2} \t{3} \t{4} \t{5} \t{6} \t{7} \t{8} \t{9}\n",
                    "NULL", "NULL", "NULL",
                    "NULL", "NULL", "NULL",
                    "NULL", navigator.transform.position.x, navigator.transform.position.z, Camera.main.transform.eulerAngles.y);
                }
                else
                {
                    Output.excelPathBuffer += string.Format("\t{0} \t{1} \t{2} \t{3} \t{4} \t{5} \t{6} \t{7} \t{8} \t{9}\n",
                    current.name, current.transform.GetSiblingIndex(), current.transform.position.x,
                    current.transform.position.z, navigator.transform.position.x,
                    navigator.transform.position.z, Vector2.Distance(new Vector2(current.transform.position.x, current.transform.position.z), new Vector2(navigator.transform.position.x, navigator.transform.position.z)), navigator.transform.position.x, navigator.transform.position.z, Camera.main.transform.eulerAngles.y);
                }

                //file writing code
                using (StreamWriter swe = File.AppendText(Output.accessibleExcelPathPath))
                {
                    //write the path buffer to the path file
                    swe.WriteLine(Output.excelPathBuffer);
                }
                //clear path buffer
                Output.excelPathBuffer = "";

            }
        }
    //End of if(endOfTrial) conditional

    NOTE: THE PATH CODE SHOULD GO BELOW THE END OF TRIAL CONDITIONAL OR YOU WILL WRITE THE SAME LAST LINE TWICE

    //End of Update
     */
}
