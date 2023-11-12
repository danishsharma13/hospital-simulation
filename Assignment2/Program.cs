// Name: Danish Sharma
// StudentID: 0623392


// class Patient
// Summary: The Patient class is used to represent a person and their emergency details
using System.Reflection.Emit;

class Patient
{
    // Data members - Getter and Private Setter
    public int PatientNumber { get; private set; }
    public int LevelOfEmergency { get; private set; }
    public int TreatmentTime { get; private set; }

    // Summary: 2-args constructor that creates a Patient object and takes in the
    //          arguments of int PatientNumber and int mean of TreatmentTime
    public Patient(int patientNumber, int meanTreatment)
    {
        this.PatientNumber = patientNumber;
        // Generate Level Of Emergency for the patient
        this.LevelOfEmergency = this.GenerateLevelOfEmergency();
        // Generate Treatment time for the paitent based on 
        // level of emergency
        this.TreatmentTime = this.GenerateTreatmentTime(meanTreatment);
    }

    // Summary: Private helper method that generates Patient's level of emergency
    private int GenerateLevelOfEmergency()
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

    // Summary: Private helper method that generates treatment time for patient
    private int GenerateTreatmentTime(int meanTreatment)
    {
        // Generate a random number to use it as "u" in T ln(u)
        Random random = new Random();
        double randomNumber = random.NextDouble();
        
        // Get the T ln(u)
        double logMeanTime = -1 * meanTreatment * Math.Log(randomNumber);

        // If level == 1 then return logMeanTime
        // If level == 2 then return 2 * logMeanTime
        // If level == 3 then return 4 * logMeanTime
        if (LevelOfEmergency == 1) return (int)logMeanTime;
        else if (LevelOfEmergency == 2) return 2 * (int)logMeanTime;
        else return 4 * (int)logMeanTime;
    }

    // Summary: Removing treatment time from patient's treatment
    // (mainly used when a treatment needs to swapped with higher emergency level)
    public void ReduceTreatmentTime(int reducedSeconds)
    {
        this.TreatmentTime -= reducedSeconds;
    }
}

// -------------------------------------------------------------------------------------

// Enum class EventType
// Summary: Event type is an enum class that will store paitent's event type
enum EventType
{
    ARRIVAL,
    DEPARTURE
}

// -------------------------------------------------------------------------------------

// class Event
// Summary: Event is a class that will store paitent's information as well as the
//          information about the event, such as type of event, doctor assigned,
//          time of event.
class Event : IComparable
{
    // Data members - Getter and Private Setter
    public Patient Patient { get; private set; }
    public EventType Type { get; private set; }
    public int DoctorAssigned { get; private set; }
    public int EventTime { get; private set; }


    // Summary: 4-args constructor that creates an Event object to populate the object's
    //          data members with appropriate information
    public Event(Patient patient, EventType type, int doctorAssigned, int eventTime)
    {
        // Populate data members with appropriate information about the Event
        this.Patient = patient;
        this.Type = type;
        // If type is departure then we assign the doctor
        // Else we store -1 to doctors assigned
        if (type == EventType.DEPARTURE) this.DoctorAssigned = doctorAssigned;
        else this.DoctorAssigned = -1;
        this.EventTime = eventTime;
    }

    // Summary: CompareTo from IComparable interface will compare timing of 2
    //          Events
    public int CompareTo(Object? obj)
    {
        // If obj is not null then compare
        // Else throw exception
        if (obj != null)
        {
            Event other = (Event)obj;

            // If current object's eventtime is less than other, then return 1
            // If current object's eventtime is greater than other, then return 0
            // Else both object have same time, return 0
            if (EventTime < other.EventTime) return 1;
            else if (EventTime > other.EventTime) return -1;
            else return 0;
        }
        else
            throw new InvalidOperationException("Cannot compare Null Object with Event Object!");
    }
}

// -------------------------------------------------------------------------------------

// class Priority Queue
// Implementation:  Binary heap ***(Professor's code)***
// Summary: Priority Queue class is taken from blackboard as per the assignment's
//          criteria, it stores future events that is prioritized by paitient's
//          arrival.
// Note: Since we are allowed to use PriorityQueue from blackboard, I have
//       edited the class to work with Event class as well as implemented
//       PeekAt(int) and RemoveAt(int) methods.

public class PriorityQueue<T> where T : IComparable
{
    private T[] A;         // Linear array of items (Generic)
    private int capacity;  // Maximum number of items in a priority queue
    private int count;     // Number of items in the priority queue

    // Summary: 0-args  constructor to create the PriorityQueue object
    //          Create an empty priority queue with length 5
    // Time complexity:  O(1)

    public PriorityQueue()
    {
        capacity = 5;
        A = new T[capacity + 1];  // Indexing begins at 1
        MakeEmpty();
    }

    // MakeEmpty
    // Reset a priority queue to empty
    // Time complexity:  O(1)

    public void MakeEmpty()
    {
        count = 0;
    }

    // Empty
    // Return true if the priority is empty; false otherwise
    // Time complexity:  O(1)

    public bool Empty()
    {
        return count == 0;
    }

    // Size
    // Return the number of items in the priority queue
    // Time complexity:  O(1)

    public int Size()
    {
        return count;
    }

    // DoubleCapacity
    // Doubles the capacity of the priority queue
    // Time complexity:  O(n)

    private void DoubleCapacity()
    {
        T[] oldA = A;

        capacity = 2 * capacity;
        A = new T[capacity + 1];
        for (int i = 1; i <= count; i++)
        {
            A[i] = oldA[i];
        }
    }

    // PercolateUp
    // Percolate up an item from position i in a priority queue
    // Time complexity:  O(log n)

    private void PercolateUp(int i)
    {
        int child = i, parent;

        // As long as child is not the root (i.e. has a parent)
        while (child > 1)
        {
            parent = child / 2;

            if (A[child].CompareTo(A[parent]) > 0)
            // If child has a higher priority than parent
            {
                // Swap parent and child
                T item = A[child];
                A[child] = A[parent];
                A[parent] = item;
                child = parent;  // Move up child index to parent index
            }
            else
                // Item is in its proper position
                return;
        }
    }

    // Insert
    // Insert an item into the priority queue
    // Time complexity:  O(log n)

    public void Insert(T item)
    {
        if (count == capacity)
        {
            DoubleCapacity();
        }
        A[++count] = item;      // Place item at the next available position
        PercolateUp(count);
    }

    // PercolateDown
    // Percolate down an item from position i in a priority queue
    // Time complexity:  O(log n)

    private void PercolateDown(int i)
    {
        int parent = i, child;

        // while parent has at least one child
        while (2 * parent <= count)
        {
            // Select the child with the highest priority
            child = 2 * parent;    // Left child index
            if (child < count)  // Right child also exists
                if (A[child + 1].CompareTo(A[child]) > 0)
                    // Right child has a higher priority than left child
                    child++;

            // If child has a higher priority than parent
            if (A[child].CompareTo(A[parent]) > 0)
            {
                // Swap parent and child
                T item = A[child];
                A[child] = A[parent];
                A[parent] = item;
                parent = child;  // Move down parent index to child index
            }
            else
                // Item is in its proper place
                return;
        }
    }

    // Remove
    // Remove (if possible) the item with the highest priority
    // Otherwise throw an exception
    // Time complexity:  O(log n)

    public void Remove()
    {
        if (Empty())
            throw new InvalidOperationException("Priority queue is empty");
        else
        {
            // Remove item with highest priority (root) and
            // replace it with the last item
            A[1] = A[count--];

            // Percolate down the new root item
            PercolateDown(1);
        }
    }

    // Front
    // Return (if possible) the item with the highest priority
    // Otherwise throw an exception
    // Time complexity:  O(1)

    public T Front()
    {
        if (Empty())
            throw new InvalidOperationException("Priority queue is empty");
        else
            return A[1];  // Return the root item (highest priority)
    }

    // BuildHeap
    // Create a binary heap from an arbitrary array
    // Time complexity:  O(n)
    private void BuildHeap()
    {
        int i;

        // Percolate down from the last parent to the root (first parent)
        for (i = count / 2; i >= 1; i--)
        {
            PercolateDown(i);
        }
    }

    // HeapSort
    // Sort and return inputArray
    // Time complexity:  O(n log n)
    public void HeapSort(T[] inputArray)
    {
        int i;

        capacity = count = inputArray.Length;

        // Copy input array to A (indexed from 1)
        for (i = capacity - 1; i >= 0; i--)
        {
            A[i + 1] = inputArray[i];
        }

        // Create a binary heap
        BuildHeap();

        // Remove the next item and place it into the input (output) array
        for (i = 0; i < capacity; i++)
        {
            inputArray[i] = Front();
            Remove();
        }
    }

    // Summary: PeekAt(int) method returns an item at a certain index
    //          within the priority queue heap
    public T PeekAt(int i)
    {
        // If i is less 1 or greater than count then throw exception
        // Since indexing starts at 1 and not 0
        if (i < 1 || i > count)
        {
            throw new ArgumentOutOfRangeException("The index is out of range, please enter valid index!");
        }

        return A[i];
    }


    // Summary: RemoveAt(int) method removes an item at a certain index
    //          within the priority queue heap
    public void RemoveAt(int i)
    {
        // If i is less 1 or greater than count then throw exception
        // Since indexing starts at 1 and not 0
        if (i < 1 || i > count)
        {
            throw new ArgumentOutOfRangeException("Index is out of range.");
        }

        // Replace the deleted item with the last item in the heap
        A[i] = A[count--];

        // Since an item is removed from the middle (possibily) we need to 
        // run Perolate Down and Up to make sure Priority Queue is not
        // violating its data structure
        PercolateDown(i);
        PercolateUp(i);
    }

}

// -------------------------------------------------------------------------------------

// class Simulation
// Summary: Simulation class is used to run a simulation based on treatment time, arrival
//          time of paitents, and number of doctors available
class Simulation
{
    // Data members
    private PriorityQueue<Event>? Priority;
    private Queue<Event>? waitingQueue1;
    private Queue<Event>? waitingQueue2;
    private Queue<Event>? waitingQueue3;

    // Summary: 0-args constructor to initialize the data members
    public Simulation()
    {
        this.Priority = new PriorityQueue<Event>();
        this.waitingQueue1 = new Queue<Event>();
        this.waitingQueue2 = new Queue<Event>(); 
        this.waitingQueue3 = new Queue<Event>();
    }

    // Summary: RunSimulation(int, int, int) will run the simulation based on the 
    //          treatment time, arrival time, and number of doctors available
    public void RunSimulation(int meanTreatment, int meanArrival, int numberOfdoctors)
    {
        // GeneratePatients in PriorityQueue
        GeneratePatients(meanTreatment, meanArrival);

        // Create array of doctors (0s) where
        // 0: available, 1-3: treating emergency level
        int[] doctorStatus = new int[numberOfdoctors];

        // currentTime variable that starts from 9am (0900), seconds 32,400
        int currentTime = 32400;

        // While loop to run simulation that will take arrival of patients and store departure
        while (this.Priority.Size() > 0)
        {
            // Get the currentEvent and remove from Priority
            Event currentEvent = Priority.Front();
            Priority.Remove();

            // Set currentEvent time to currentTime since the "that" much time is passed
            currentTime = currentEvent.EventTime;

            // If the currentEvent type is ARRIVAL then we either find a doctor or put the
            // patient in waiting queue
            if (currentEvent.Type == EventType.ARRIVAL)
            {
                // Get index of available doctors
                int availableDoctorIndex = AvailableDoctor(doctorStatus, currentEvent.Patient.LevelOfEmergency);

                // If there no doctor available/cant pre-empt, then index is -1, add patient to waiting queue
                // If there is doctors available, then check level doctor is operating or if doctor is available (0)
                if (availableDoctorIndex == -1)
                {
                    // Add the patient to waitingQueue
                    if (currentEvent.Patient.LevelOfEmergency == 3) waitingQueue3.Enqueue(currentEvent);
                    else if (currentEvent.Patient.LevelOfEmergency == 2) waitingQueue2.Enqueue(currentEvent);
                    else if (currentEvent.Patient.LevelOfEmergency == 1) waitingQueue1.Enqueue(currentEvent);
                }
                else
                {
                    if (doctorStatus[availableDoctorIndex] == 0)
                    {
                        // Assign the patient to the doctor, start departure event and store it in the queue
                        doctorStatus[availableDoctorIndex] = currentEvent.Patient.LevelOfEmergency;
                        Event departureEvent = new Event(currentEvent.Patient, EventType.DEPARTURE, 
                            availableDoctorIndex + 1, currentTime + currentEvent.Patient.TreatmentTime);
                        Priority.Insert(departureEvent);
                    }
                    else if (doctorStatus[availableDoctorIndex] > 0)
                    {
                        // Find the departure event based on doctor treating the patient
                        for (int i = 1; i < Priority.Size(); i++)
                        {
                            if (Priority.PeekAt(i).DoctorAssigned == (availableDoctorIndex + 1))
                            {
                                // Update patient and reduce the treatement time and then create
                                // arrival event and add the patient to the queue
                                Patient previousPatient = Priority.PeekAt(i).Patient;
                                previousPatient.ReduceTreatmentTime(Priority.PeekAt(i).EventTime - currentTime);
                                Event previousEvent = new Event(previousPatient, EventType.ARRIVAL, -1, currentTime);
                                
                                // Put previousEvent into appropriate waiting queues
                                if (Priority.PeekAt(i).Patient.LevelOfEmergency == 3)
                                    waitingQueue3.Enqueue(previousEvent);
                                else if (Priority.PeekAt(i).Patient.LevelOfEmergency == 2)
                                    waitingQueue2.Enqueue(previousEvent);
                                else if (Priority.PeekAt(i).Patient.LevelOfEmergency == 1)
                                    waitingQueue1.Enqueue(previousEvent);
                                
                                Priority.RemoveAt(i);
                                break;
                            }
                        }

                        // Assign the patient to the doctor, start departure event and store it in the queue
                        doctorStatus[availableDoctorIndex] = currentEvent.Patient.LevelOfEmergency;
                        Event departureEvent = new Event(currentEvent.Patient, EventType.DEPARTURE, 
                            availableDoctorIndex + 1, currentTime + currentEvent.Patient.TreatmentTime);
                        Priority.Insert(departureEvent);
                    }
                }
            }
        }


    }

    // Summary: Private helper function that returns the index of doctor that is available
    //          and based on emergency level
    private int AvailableDoctor(int[] doctors, int level)
    {
        // For loop for available doctor (0)
        for (int i = 0; i < doctors.Length; i++)
        {
            if (doctors[i] == 0)
            {
                return i;
            }
        }

        // For loop for busy doctor treating lower level
        for (int i = 0; i < doctors.Length; i++)
        {
            if (doctors[i] < level)
            {
                return i;
            }
        }

        return -1;
    }

    // Summary: Private helper GeneratePatients(int, int) generates patients' arrival in
    //          the PriorityQueue based on the meanTreatment and meanArrival
    private void GeneratePatients(int meanTreatment, int meanArrival)
    {
        // currentTime variable that starts from 9am (0900), seconds 32,400
        int currentTime = 32400;
        int patientNumber = 1;

        // Creating arrival events in the PriorityQueue
        while (currentTime <= 54000)
        {
            // Creating random numbers around the meanArrival (+-20)
            Random random = new Random();
            int randomArrival = random.Next(meanArrival - 20, meanArrival + 20);

            // If currentTime plus randomArrival is less then 54000 (3pm) then create patient
            // and insert the patient in priority queue
            if ((currentTime + randomArrival) <= 54000)
            {
                Event arrivalEvent = new Event(new Patient(patientNumber, meanTreatment), EventType.ARRIVAL, -1, currentTime + randomArrival);
                this.Priority.Insert(arrivalEvent);
            }

            // Adding randomArrival to the currentTime so we do not take patients after
            // 3pm (1500), seconds 54,000
            currentTime += randomArrival;

            patientNumber++;
        }
    }

    // Summary: ConvertSeconds(int) is used to convert the int seconds into
    //          readable string that is used to print the events
    public string ConvertSeconds(int seconds)
    {
        // 1 hour = 3600 seconds, so dividing it will give us the hour
        int hours = seconds / 3600;
        // 1 minute is 60 seconds, but we need to remove the hour from it
        // so we seconds mod 3600 to get the remaining seconds and divide
        // from 60
        int minutes = (seconds % 3600) / 60;
        // To get seconds we can just seconds mod 60 so we can have the 
        // seconds remaining after taking away hours and minutes from it
        int sec = seconds % 60;

        return $"{hours:D2}:{minutes:D2}:{sec:D2}";
    }
}


class Program
{
    public static void Main(String[] args)
    {
        /*PriorityQueue<Event>? priorityQueue = new PriorityQueue<Event>();

        // currentTime variable that starts from 9am (0900), seconds 32,400
        int currentTime = 32400;
        int patientNumber = 0;

        // Creating arrival events in the PriorityQueue
        while (currentTime <= 54000)
        {
            // Creating random numbers around the meanArrival (+-20)
            Random random = new Random();
            int randomArrival = random.Next(600 - 20, 600 + 20);

            // If currentTime plus randomArrival is less then 54000 (3pm) then create patient
            // and insert the patient in priority queue
            if ((currentTime + randomArrival) <= 54000)
            {
                Event arrivalEvent = new Event(new Patient(patientNumber, 300), EventType.ARRIVAL, -1, currentTime + randomArrival);
                priorityQueue.Insert(arrivalEvent);
            }

            // Adding randomArrival to the currentTime so we do not take patients after
            // 3pm (1500), seconds 54,000
            currentTime += randomArrival;

            patientNumber++;
        }

        while (priorityQueue.Size() > 0)
        {
            Console.WriteLine("Patient(" + priorityQueue.Front().Patient.PatientNumber + ") arrived at " + priorityQueue.Front().EventTime + " Level(" + priorityQueue.Front().Patient.LevelOfEmergency + ")");
            priorityQueue.RemoveAt(1);
        }
*/
        /*int i = 0;
        while (i < 20)
        {
            Patient p = new Patient(i, 300);
            Console.WriteLine("Patient(" + p.PatientNumber + ") - Level: " + p.LevelOfEmergency + ", Treatment: " + p.TreatmentTime);
            i++;
        }*/

    }
}