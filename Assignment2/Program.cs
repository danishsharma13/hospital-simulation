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
    public double EventTime { get; private set; }


    // Summary: 4-args constructor that creates an Event object to populate the object's
    //          data members with appropriate information
    public Event(Patient patient, EventType type, int doctorAssigned, double eventTime)
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
// Implementation:  Binary heap
// Summary: Priority Queue class is taken from blackboard as per the assignment's
//          criteria, it stores future events that is prioritized by paitient's
//          arrival.

public class PriorityQueue<T>
{
    private T[] A;         // Linear array of items (Generic)
    private int capacity;  // Maximum number of items in a priority queue
    private int count;     // Number of items in the priority queue

    // Constructor 1
    // Create an empty priority queue
    // Time complexity:  O(1)

    public PriorityQueue()
    {
        capacity = 3;
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
}