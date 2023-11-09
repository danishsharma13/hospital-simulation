// Name: Danish Sharma
// StudentID: 0623392


// class Patient
// Summary: The Patient class is used to represent a person and their emergency details
class Patient
{
    // Data members
    private int patient_number;
    private int level_of_emergency;
    private double treatment_time;

    // Get PatientNumber
    public int PatientNumber
    {
        get { return patient_number; }
    }

    // Get LevelOfEmergency
    public int LevelOfEmergency
    { 
        get { return level_of_emergency;} 
    }

    // Get and Set TreatmentTime
    public double TreatmentTime
    {
        get { return treatment_time;}
        set { treatment_time = value; }
    }

    // Summary: 2-args constructor that creates a Patient object and takes in the
    //          arguments of int PatientNumber and int mean of TreatmentTime
    public Patient(int patientNumber, int meanTime)
    {
        this.patient_number = PatientNumber;
        // Generate Level Of Emergency for the patient
        this.level_of_emergency = GenerateLevelOfEmergency();


    }

    // Summary: Helper method that generates Patient's level of emergency
    public int GenerateLevelOfEmergency()
    {
        // Generate a random number to convert it into Level Of Emergency
        Random random = new Random();
        double randomNumber = random.NextDouble();

        // If randomNumber is less than 0.6 then level = 1
        // If randomNumber is greater than 0.6 and less than 0.9 then level = 2
        // If randomNumber is greater than 0.9 then level = 3
        if (randomNumber < 0.6) return 1;
        else if (randomNumber < 0.9) return 2;
        else return 3;
    }

    // Summary: Helper method that generates treatment time for patient
    public double GenerateTreatmentTime(int meanTime)
    {
        // Generate a random number to use it as "u" in T ln(u)
        Random random = new Random();
        double randomNumber = random.NextDouble();
        
        // Get the T ln(u)
        double logMeanTime = meanTime * Math.Log(randomNumber);

        // If level == 1 then return logMeanTime
        // If level == 2 then return 2 * logMeanTime
        // If level == 3 then return 4 * logMeanTime
        if (LevelOfEmergency == 1) return logMeanTime;
        else if (LevelOfEmergency == 2) return 2 * logMeanTime;
        else return 4 * logMeanTime;
    }

    // Summary: Removing treatment time from patient's treatment
    // (mainly used when a treatment needs to swapped with higher emergency level)
    public void ReduceTreatmentTime(double reducedSeconds)
    {
        this.treatment_time -= reducedSeconds;
    }

}