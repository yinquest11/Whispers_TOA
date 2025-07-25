﻿
using System.Collections.Generic;
using UnityEngine;

public class RopeVerlet2 : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public float ropeSegLen = 0.25f; // distance between 2 point
    public int segmentlength = 35; // how many point
    public Vector2 gravityForce = new Vector2(0f, 0f);

    private float lineWidth = 0.1f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for(int i = 0; i < segmentlength; ++i)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint)); // use current ropeStartPoint as this segment's head position
            ropeStartPoint.y -= ropeSegLen; //  move ropeStartPoint down for the next segment
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    public void Simulate() // Let the rope move ! use Verlet Integration update every point position, simulate physic
    {
        // Define  a Gravity force
        //Vector2 forceGravity = new Vector2(0f, -1f);


        for(int i = 0; i < this.segmentlength; ++i)
        {
            RopeSegment eachSegment = this.ropeSegments[i]; // loop each point
            Vector2 velocity = eachSegment.posNow - eachSegment.posOld; // calculate the delta position now - old
            eachSegment.posOld = eachSegment.posNow; // next old posiiton is my current position first
            eachSegment.posNow += velocity; // then update my current position += the delta ( applying the formula)
            eachSegment.posNow += gravityForce * Time.deltaTime; // Applying gravity, a distance should fall down cause by gravity
            this.ropeSegments[i] = eachSegment; // update the segment in the list with its newPos and oldPos information
        }

        // Constraints
        for(int i = 0; i < 50; ++i)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint() // apply constraint to each individual point
    {
        // Constarints ( First segment alaways follow mosue position )

        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Define the first point position
        this.ropeSegments[0] = firstSegment;

        // Keep distance
        for(int i = 0; i < this.segmentlength - 1; ++i)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            
            Vector2 delta = firstSeg.posNow - secondSeg.posNow;  // Calculate the Vector2 between current i point and its next point
            
            
            if (delta.magnitude == 0f) continue;  // if the length of the Vector2 is 0 (no error, excatly at the correct position), continue 

            float error = delta.magnitude - ropeSegLen;  // calculate the different legth, delta.magnitue is our actual punya, ropeSegLen is should punya
          
            Vector2 changeAmount = delta.normalized * error; // Give the error length a direction (delta),  

            // applying the error value to current and its next point

            
            if (i == 0) // If this is the first segment (anchored), only move the second point to correct the distance error
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
            //else if (i == ropeSegments.Count - 2)
            //{
            //    firstSeg.posNow -= changeAmount;
            //    ropeSegments[i] = firstSeg;

            //}
            else // else, both of them move away or to the center by half of the error to match the expected  length
            {
                                                        // for the relativly first point  
                firstSeg.posNow -= changeAmount * 0.5f; // positive error mean current length is over long, first point should go behind using -
                ropeSegments[i] = firstSeg;             // negative error mean current length is shotter than expected, first point -- = +, go forwards


                secondSeg.posNow += changeAmount * 0.5f; // for the relativly second point  
                ropeSegments[i + 1] = secondSeg;         // positive error mean current length is over long, second point should go forwards using +
                                                         // neegative error mean current lenth is over short, second point should go behind using -
            }

            // 把粒子修正过了的位置数据储存回去我们自己的数据结构，用 lineRenderer.SetPositions(ropePositions); 实现新的位置


        }
        //RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
        //lastSegment.posNow = Vector2.zero;
        //ropeSegments[ropeSegments.Count - 1] = lastSegment;

    }


    private void DrawRope() // Take the posNow each frame to draw the position
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Collect all segement position 
        Vector3[] ropePositions = new Vector3[this.segmentlength]; // the all position of the segment in the line is on this array

        for (int i = 0; i < this.segmentlength; ++i)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        // set lineRenderer point accroding the posNow array
        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }

    }

}
