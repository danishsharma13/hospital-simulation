using System;
using System.Collections.Generic;
using System.Linq;

class Patient
{
    public int PatientNumber { get; set; }
    public int LevelOfEmergency { get; set; }
    public double TreatmentTime { get; set; }
    public double ArrivalTime { get; set; }
    public double WaitTime { get; set; } // To record patient's waiting time
}

class Event : IComparable<Event>
{
    public Patient Patient { get; }
    public EventType Type { get; set; }
    public double Time { get; set; }

    public Event(Patient patient, EventType type, double time)
    {
        Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null");
        Type = type;
        Time = time;
    }

    public int CompareTo(Event other)
    {
        return Time.CompareTo(other.Time);
    }
}

enum EventType
{
    Arrival,
    Departure
}

class Simulation
{
    private List<Queue<Patient>> waitingQueues;
    private List<bool> doctorAvailability; // To track doctor availability
    private List<Event> eventQueue = new List<Event>();

    public Simulation(int numberOfDoctors)
    {
        waitingQueues = new List<Queue<Patient>>();
        doctorAvailability = new List<bool>();

        for (int i = 0; i < 3; i++)
        {
            waitingQueues.Add(new Queue<Patient>());
        }

        for (int i = 0; i < numberOfDoctors; i++)
        {
            doctorAvailability.Add(true); // Initialize all doctors as available
        }
    }

    public void Run(double meanTreatmentTime, double meanInterArrivalTime, int minPatients, int maxPatients)
    {
        Random random = new Random();
        List<Patient> patients = new List<Patient>(); // To store patients for performance measurement
        int patientsProcessed = 0; // Counter for processed patients

        while (eventQueue.Count > 0 && (patientsProcessed < maxPatients || patientsProcessed < minPatients))
        {
            eventQueue.Sort();
            Event currentEvent = eventQueue[0];
            eventQueue.RemoveAt(0);

            if (currentEvent.Type == EventType.Arrival)
            {
                Patient patient = currentEvent.Patient;
                patients.Add(patient); // Store the patient for performance measurement

                int availableDoctor = FindAvailableDoctor();
                if (availableDoctor >= 0)
                {
                    AssignPatientToDoctor(patient, availableDoctor);
                    double departureTime = currentEvent.Time + patient.TreatmentTime;
                    Event departureEvent = new Event(patient, EventType.Departure, departureTime);
                    eventQueue.Add(departureEvent);
                    Console.WriteLine($"Patient {patient.PatientNumber} (Level {patient.LevelOfEmergency}) has arrived and assigned to Doctor {availableDoctor}.");
                }
                else
                {
                    waitingQueues[patient.LevelOfEmergency - 1].Enqueue(patient);
                    patient.WaitTime = currentEvent.Time - patient.ArrivalTime; // Calculate waiting time
                    Console.WriteLine($"Patient {patient.PatientNumber} (Level {patient.LevelOfEmergency}) has arrived and is waiting.");
                }

                double interArrivalTime = -meanInterArrivalTime * Math.Log(random.NextDouble());
                double nextArrivalTime = currentEvent.Time + interArrivalTime;
                Patient nextPatient = GenerateRandomPatient(meanTreatmentTime);
                nextPatient.ArrivalTime = currentEvent.Time; // Record patient arrival time
                Event nextArrivalEvent = new Event(nextPatient, EventType.Arrival, nextArrivalTime);
                eventQueue.Add(nextArrivalEvent);

                // Increment the counter for processed patients
                patientsProcessed++;
            }
            else if (currentEvent.Type == EventType.Departure)
            {
                int levelOfEmergency = currentEvent.Patient.LevelOfEmergency;
                if (waitingQueues[levelOfEmergency - 1].Count > 0)
                {
                    Patient waitingPatient = waitingQueues[levelOfEmergency - 1].Dequeue();
                    AssignPatientToDoctor(waitingPatient, levelOfEmergency);
                    double departureTime = currentEvent.Time + waitingPatient.TreatmentTime;
                    Event departureEvent = new Event(waitingPatient, EventType.Departure, departureTime);
                    eventQueue.Add(departureEvent);
                    Console.WriteLine($"Patient {waitingPatient.PatientNumber} (Level {waitingPatient.LevelOfEmergency}) has departed from Doctor {levelOfEmergency}.");
                }
                else
                {
                    doctorAvailability[levelOfEmergency - 1] = true; // Make the doctor available
                    Console.WriteLine($"Doctor {levelOfEmergency} is now available.");
                }
            }
        }

        // Calculate and output performance measures for the processed patients
        foreach (var patient in patients)
        {
            if (patient.WaitTime >= 0)
            {
                Console.WriteLine($"Patient {patient.PatientNumber} (Level {patient.LevelOfEmergency}) - Waiting Time: {patient.WaitTime} seconds");
            }
            else
            {
                Console.WriteLine($"Patient {patient.PatientNumber} (Level {patient.LevelOfEmergency}) - Left without treatment due to no available doctor.");
            }
        }
    }

    private int FindAvailableDoctor()
    {
        for (int i = 0; i < doctorAvailability.Count; i++)
        {
            if (doctorAvailability[i])
            {
                doctorAvailability[i] = false; // Set the doctor as busy
                return i; // Return the doctor index
            }
        }
        return -1; // No available doctor
    }

    private void AssignPatientToDoctor(Patient patient, int doctor)
    {
        if (doctorAvailability.Count > doctor && doctorAvailability[doctor])
        {
            // The specified doctor is available, assign the patient
            doctorAvailability[doctor] = false; // Set the doctor as busy
            patient.WaitTime = 0; // The patient did not have to wait
        }
        else
        {
            // The specified doctor is not available, search for the next available doctor
            int nextAvailableDoctor = FindNextAvailableDoctor(doctor);

            if (nextAvailableDoctor != -1)
            {
                // Assign the patient to the next available doctor
                doctorAvailability[nextAvailableDoctor] = false; // Set the doctor as busy
                patient.WaitTime = 0; // The patient did not have to wait
            }
            else
            {
                // Handle the case where no doctor is available
                patient.WaitTime = -1; // Indicate that the patient could not be assigned to a doctor
            }
        }
    }

    private int FindNextAvailableDoctor(int currentDoctor)
    {
        int nextAvailableDoctor = currentDoctor + 1;

        while (nextAvailableDoctor != currentDoctor)
        {
            if (nextAvailableDoctor >= doctorAvailability.Count)
            {
                nextAvailableDoctor = 0; // Wrap around to the first doctor
            }

            if (doctorAvailability[nextAvailableDoctor])
            {
                return nextAvailableDoctor; // Found an available doctor
            }

            nextAvailableDoctor++;
        }

        return -1; // No available doctor found
    }

    private Patient GenerateRandomPatient(double meanTreatmentTime)
    {
        Random random = new Random();
        Patient patient = new Patient
        {
            PatientNumber = random.Next(1, 1000),
            LevelOfEmergency = DetermineLevelOfEmergency(random),
            ArrivalTime = 0
        };

        double scalingFactor = DetermineScalingFactor(patient.LevelOfEmergency);
        patient.TreatmentTime = -meanTreatmentTime * scalingFactor * Math.Log(random.NextDouble());

        return patient;
    }

    private int DetermineLevelOfEmergency(Random random)
    {
        double r = random.NextDouble();
        if (r < 0.6)
            return 1;
        else if (r < 0.9)
            return 2;
        else
            return 3;
    }

    private double DetermineScalingFactor(int levelOfEmergency)
    {
        if (levelOfEmergency == 2)
            return 2;
        if (levelOfEmergency == 3)
            return 4;
        return 1;
    }
}

class Program
{
    static void Main()
    {
        int numberOfDoctors = 5;
        Simulation simulation = new Simulation(numberOfDoctors);

        double meanTreatmentTime = 15;
        double meanInterArrivalTime = 10;
        int minPatients = 10; // Specify the minimum number of patients
        int maxPatients = 50; // Specify the maximum number of patients

        simulation.Run(meanTreatmentTime, meanInterArrivalTime, minPatients, maxPatients);
    }
}
