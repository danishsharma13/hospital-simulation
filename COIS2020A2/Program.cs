using System;
using System.Collections.Generic;

class Patient
{
    public int PatientNumber { get; set; }
    public int EmergencyLevel { get; set; }
    public double TreatmentTime { get; set; }

    public Patient(int patientNumber, int emergencyLevel, double treatmentTime)
    {
        PatientNumber = patientNumber;
        EmergencyLevel = emergencyLevel;
        TreatmentTime = treatmentTime;
    }
}

class Event : IComparable<Event>
{
    public Patient Patient { get; set; }
    public EventType Type { get; set; }
    public double Time { get; set; }
    public int DoctorAssigned { get; set; }

    public Event(Patient patient, EventType type, double time, int doctorAssigned)
    {
        Patient = patient;
        Type = type;
        Time = time;
        DoctorAssigned = doctorAssigned;
    }

    public int CompareTo(Event? other)
    {
        return Time.CompareTo(other?.Time ?? 0);
    }
}

enum EventType
{
    Arrival,
    Departure
}

class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public PriorityQueue()
    {
        data = new List<T>();
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (data[ci].CompareTo(data[pi]) >= 0) break;
            Swap(ci, pi);
            ci = pi;
        }
    }

    public T Dequeue()
    {
        int li = data.Count - 1;
        if (li < 0) return default(T)!;

        T frontItem = data[0];
        data[0] = data[li];
        data.RemoveAt(li);

        --li;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0) break;
            Swap(pi, ci);
            pi = ci;
        }
        return frontItem;
    }

    public T Peek()
    {
        return data.Count > 0 ? data[0] : default(T)!;
    }

    public int Count()
    {
        return data.Count;
    }

    private void Swap(int i, int j)
    {
        T tmp = data[i];
        data[i] = data[j];
        data[j] = tmp;
    }
}

class Simulation
{
    private readonly int NumberOfDoctors;
    private readonly double AverageArrivalTime;
    private readonly double TreatmentTimeT;
    private readonly int NumberOfRuns = 10;

    private PriorityQueue<Event>? eventQueue;
    private Random random = new Random();

    public Simulation(int numberOfDoctors, double averageArrivalTime, double treatmentTimeT)
    {
        NumberOfDoctors = numberOfDoctors;
        AverageArrivalTime = averageArrivalTime;
        TreatmentTimeT = treatmentTimeT;
    }

    public double RunSimulation()
    {
        double totalWaitTime = 0;

        for (int run = 0; run < NumberOfRuns; run++)
        {
            eventQueue = new PriorityQueue<Event>();
            Queue<Event>[] waitingQueues = new Queue<Event>[3];

            for (int i = 0; i < waitingQueues.Length; i++)
            {
                waitingQueues[i] = new Queue<Event>();
            }

            double currentTime = 0;
            int patientNumber = 1;

            while (currentTime <= 360)
            {
                double interarrivalTime = GenerateExponential(AverageArrivalTime);
                currentTime += interarrivalTime;

                int emergencyLevel = GenerateEmergencyLevel();
                double treatmentTime = GenerateExponential(TreatmentTimeT * Math.Pow(2, emergencyLevel - 1));

                var arrivalEvent = new Event(new Patient(patientNumber, emergencyLevel, treatmentTime), EventType.Arrival, currentTime, 0);
                eventQueue.Enqueue(arrivalEvent);
                patientNumber++;
            }

            int doctorIndex = 0;
            while (eventQueue.Count() > 0)
            {
                Event currentEvent = eventQueue.Dequeue();

                if (currentEvent.Type == EventType.Arrival)
                {
                    int availableDoctor = FindAvailableDoctor(doctorIndex);
                    if (availableDoctor >= 0)
                    {
                        currentEvent.DoctorAssigned = availableDoctor;
                        currentEvent.Type = EventType.Departure;
                        currentEvent.Time += currentEvent.Patient.TreatmentTime;
                        eventQueue.Enqueue(currentEvent);
                    }
                    else
                    {
                        waitingQueues[currentEvent.Patient.EmergencyLevel - 1].Enqueue(currentEvent);
                    }
                }
                else
                {
                    totalWaitTime += (currentEvent.Time - currentTime);
                    if (waitingQueues[currentEvent.Patient.EmergencyLevel - 1].Count > 0)
                    {
                        Event nextPatient = waitingQueues[currentEvent.Patient.EmergencyLevel - 1].Dequeue();
                        nextPatient.Type = EventType.Departure;
                        nextPatient.DoctorAssigned = currentEvent.DoctorAssigned;
                        nextPatient.Time = currentEvent.Time + nextPatient.Patient.TreatmentTime;
                        eventQueue.Enqueue(nextPatient);
                    }
                }
            }
        }

        return totalWaitTime / NumberOfRuns;
    }

    public PriorityQueue<Event>? GetEventQueue()
    {
        return eventQueue;
    }

    private double GenerateExponential(double mean)
    {
        double u = random.NextDouble();
        return -mean * Math.Log(1 - u);
    }

    private int GenerateEmergencyLevel()
    {
        double u = random.NextDouble();
        if (u < 0.6) return 1;
        if (u < 0.9) return 2;
        return 3;
    }

    private int FindAvailableDoctor(int startIndex)
    {
        for (int i = startIndex; i < NumberOfDoctors; i++)
        {
            if (i % NumberOfDoctors == startIndex && i != startIndex)
            {
                return -1; // All doctors are busy
            }
        }
        return startIndex;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int numberOfDoctors = 5;
        double averageArrivalTime = 10;
        double treatmentTimeT = 5;
        Simulation simulation = new Simulation(numberOfDoctors, averageArrivalTime, treatmentTimeT);
        double averageWaitTime = simulation.RunSimulation();

        Console.WriteLine($"Average Wait Time: {averageWaitTime:F2} minutes");

        // Access the eventQueue using the GetEventQueue method
        // Access the eventQueue using the GetEventQueue method
        PriorityQueue<Event>? eventQueue = simulation.GetEventQueue();

        if (eventQueue != null)
        {
            while (eventQueue.Count() > 0)
            {
                Event currentEvent = eventQueue.Dequeue();

                if (currentEvent.Type == EventType.Arrival)
                {
                    if (currentEvent.DoctorAssigned == 0)
                    {
                        Console.WriteLine($"{currentEvent.Time:HH:mm:ss} - Patient {currentEvent.Patient.PatientNumber} ({currentEvent.Patient.EmergencyLevel}) arrives and is seated in the waiting room.");
                    }
                    else
                    {
                        Console.WriteLine($"{currentEvent.Time:HH:mm:ss} - Patient {currentEvent.Patient.PatientNumber} ({currentEvent.Patient.EmergencyLevel}) arrives and is assigned to Doctor {currentEvent.DoctorAssigned}.");
                    }
                }
                else
                {
                    Console.WriteLine($"{currentEvent.Time:HH:mm:ss} - Doctor {currentEvent.DoctorAssigned} completes treatment of Patient {currentEvent.Patient.PatientNumber}.");
                }
            }
        }
    }
}
 