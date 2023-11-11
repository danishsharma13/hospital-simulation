// Name: Danish Sharma
// StudentID: 0623392


// class Patient
// Summary: The Patient class is used to represent a person and their emergency details
class Patient
{
    // Data members - Getter and Private Setter
    public int PatientNumber { get; private set; }
    public int LevelOfEmergency { get; private set; }
    public double TreatmentTime { get; private set; }

    // Summary: 2-args constructor that creates a Patient object and takes in the
    //          arguments of int PatientNumber and int mean of TreatmentTime
    public Patient(int patientNumber, int meanTime)
    {
        this.PatientNumber = patientNumber;
        // Generate Level Of Emergency for the patient
        this.LevelOfEmergency = GenerateLevelOfEmergency();
        // Generate Treatment time for the paitent based on 
        // level of emergency
        this.TreatmentTime = GenerateTreatmentTime(meanTime);
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
        this.TreatmentTime -= reducedSeconds;
    }
}

// -------------------------------------------------------------------------------------

// Summary: Event type is an enum class that will store paitent's event type
enum EventType
{
    ARRIVAL,
    DEPARTURE
}

// -------------------------------------------------------------------------------------

// Summary: Event is a class that will store paitent's event type
class Event
{
    public Patient Patient { get; private set; }
    public EventType Type { get; private set; }
    public int DoctorAssigned { get; private set; }
    public double EventTime { get; private set; }

    public Event(Patient patient, EventType type, int doctorAssigned, double eventTime)
    {
        Patient = patient;
        Type = type;
        DoctorAssigned = type == EventType.DEPARTURE ? doctorAssigned : -1; // -1 indicates no doctor assigned for arrivals
        EventTime = eventTime;
    }

    // Additional methods and logic can be added as needed
}