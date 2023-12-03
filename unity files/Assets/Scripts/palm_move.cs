using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class palm_move : MonoBehaviour
{
    public GameObject palm, finger_thumb, index_finger, middle_finger, ring_finger, pinky_finger;//fingers tattoo objects in scene
    public GameObject manager;//game object which runs script to receive udp data and save in variables
    public int feedW, feedH;//height and width of video feed in which hand is recognized
    string json_data;
    hand_landmark hand;//hand landmark data object
    public int palm_x, palm_y;//palm centroid coordinates, scaling and rotation factors
    public float palm_sx, palm_sy, palm_sz, palm_rx, palm_ry, palm_rz;
    public int thumb_x, thumb_y, thumb_rx=0, thumb_ry=0, thumb_rz;
    CFinger Othumb, Oindex, Omiddle, Oring, Opinky;
    public const double PI = 3.1415926535897931;
    // Start is called before the first frame update
    void Start()
    {
      palm = GameObject.Find("/Canvas/palm");
      finger_thumb = GameObject.Find("/Canvas/finger0");
      index_finger = GameObject.Find("/Canvas/finger1");
      middle_finger = GameObject.Find("/Canvas/finger2");
      ring_finger = GameObject.Find("/Canvas/finger3");
      pinky_finger = GameObject.Find("/Canvas/finger4");
      manager = GameObject.Find("/Manager");
      Othumb = new CFinger();
      Oindex = new CFinger();
      Omiddle = new CFinger();
      Oring = new CFinger();
      Opinky = new CFinger(); 
    }
    void update_palm(){
      int[] x = {hand.wrist_X, hand.thumb_cmc_X, hand.index_finger_mcp_X, hand.middle_finger_mcp_X, hand.ring_finger_mcp_X, hand.pinky_mcp_X, hand.wrist_X};
      int[] y = {hand.wrist_Y, hand.thumb_cmc_Y, hand.index_finger_mcp_Y, hand.middle_finger_mcp_Y, hand.ring_finger_mcp_Y, hand.pinky_mcp_Y, hand.wrist_Y};
      // Finding area of polygon using shoelace formula
      double area = 0;
      for(int i=0;i<6;i++){
        area += x[i]*y[i+1] - x[i+1]*y[i];
      }
      area = area/2;
      // finding centroid of polygon
      double Cx=0, Cy=0;
      for(int i=0;i<6;i++){
        Cx += (double)(x[i]+x[i+1]) * (x[i]*y[i+1] - x[i+1]*y[i]);
      }
      Cx = Cx / ((double)6*area);

      for(int i=0;i<6;i++){
        Cy += (double)(y[i]+y[i+1]) * (x[i]*y[i+1] - x[i+1]*y[i]);
      }
      Cy = Cy / ((double)6*area);

      // update palm centere coordinate as Cx,Cy
      palm_x = (int)Cx;
      palm_y = (int)Cy;

      // rotation 
      float dy = (float)(hand.wrist_Y-hand.middle_finger_mcp_Y);
      float dx = (float)(hand.wrist_X-hand.middle_finger_mcp_X);
      palm_rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);


      // scaling
      // // consideringn landmarks {wrist, middle_finger_mcp, ring_finger_mcp} to form a triangle
      // // this triangle area gives scaling factor
      // int[] triangle_x = {hand.wrist_X, hand.middle_finger_mcp_X, hand.ring_finger_mcp_X};
      // int[] triangle_y = {hand.wrist_Y, hand.middle_finger_mcp_Y, hand.ring_finger_mcp_Y};
      // // area of triangle = 1/2 * (x1*(y2-y3) + x2*(y3-y1) + x3*(y1-y2))
      // area = 0.5 * (double)(  triangle_x[0]*(triangle_y[1]-triangle_y[2])
      //                       + triangle_x[1]*(triangle_y[2]-triangle_y[0])
      //                       + triangle_x[2]*(triangle_y[0]-triangle_y[1]));
      RectTransform palm_rt = palm.GetComponent<RectTransform>();
      palm_sx = palm_sy = distance(hand.wrist_X, 
                                   hand.wrist_Y, 
                                   hand.middle_finger_mcp_X, 
                                   hand.middle_finger_mcp_Y)/((float)palm_rt.rect.height);
      

    }
    // returns distance between points p1 and p2 without converting coordinate system
    float distance(float p1_x, float p1_y, float p2_x, float p2_y){
      return Mathf.Sqrt((p1_x-p2_x)*(p1_x-p2_x) + (p1_y-p2_y)*(p1_y-p2_y));
    }
    void update_fingers(){
      // thumb finger
      Othumb.x = (hand.thumb_cmc_X + hand.thumb_mcp_X + hand.thumb_ip_X + hand.thumb_tip_X)/4;
      Othumb.y = (hand.thumb_cmc_Y + hand.thumb_mcp_Y + hand.thumb_ip_Y + hand.thumb_tip_Y)/4;
      float dy = (float)(hand.thumb_tip_Y-hand.thumb_cmc_Y);
      float dx = (float)(hand.thumb_tip_X-hand.thumb_cmc_X);
      Othumb.rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);

      Othumb.rt = finger_thumb.GetComponent<RectTransform>();
      Othumb.sx = Othumb.sy = (distance(hand.thumb_tip_X,
                                        hand.thumb_tip_Y,
                                        hand.thumb_cmc_X,
                                        hand.thumb_cmc_Y))/((float)(Othumb.rt.rect.height));
      // Debug.Log("scale = ");
      // Debug.Log(scale);


      // index finger
      Oindex.x = (hand.index_finger_mcp_X + hand.index_finger_pip_X + hand.index_finger_dip_X + hand.index_finger_tip_X)/4;
      Oindex.y = (hand.index_finger_mcp_Y + hand.index_finger_pip_Y + hand.index_finger_dip_Y + hand.index_finger_tip_Y)/4;
      dy = (float)(hand.index_finger_tip_Y-hand.index_finger_mcp_Y);
      dx = (float)(hand.index_finger_tip_X-hand.index_finger_mcp_X);
      Oindex.rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);

      Oindex.rt = index_finger.GetComponent<RectTransform>();
      Oindex.sx = Oindex.sy = (distance(hand.index_finger_tip_X,
                                        hand.index_finger_tip_Y,
                                        hand.index_finger_mcp_X,
                                        hand.index_finger_mcp_Y))/((float)(Oindex.rt.rect.height));

      // middle finger
      Omiddle.x = (hand.middle_finger_mcp_X + hand.middle_finger_pip_X + hand.middle_finger_dip_X + hand.middle_finger_tip_X)/4;
      Omiddle.y = (hand.middle_finger_mcp_Y + hand.middle_finger_pip_Y + hand.middle_finger_dip_Y + hand.middle_finger_tip_Y)/4;
      dy = (float)(hand.middle_finger_tip_Y-hand.middle_finger_mcp_Y);
      dx = (float)(hand.middle_finger_tip_X-hand.middle_finger_mcp_X);
      Omiddle.rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);

      Omiddle.rt = middle_finger.GetComponent<RectTransform>();
      Omiddle.sx = Omiddle.sy = (distance(hand.middle_finger_tip_X,
                                        hand.middle_finger_tip_Y,
                                        hand.middle_finger_mcp_X,
                                        hand.middle_finger_mcp_Y))/((float)(Omiddle.rt.rect.height));


      // ring finger
      Oring.x = (hand.ring_finger_mcp_X + hand.ring_finger_pip_X + hand.ring_finger_dip_X + hand.ring_finger_tip_X)/4;
      Oring.y = (hand.ring_finger_mcp_Y + hand.ring_finger_pip_Y + hand.ring_finger_dip_Y + hand.ring_finger_tip_Y)/4;
      dy = (float)(hand.ring_finger_tip_Y-hand.ring_finger_mcp_Y);
      dx = (float)(hand.ring_finger_tip_X-hand.ring_finger_mcp_X);
      Oring.rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);
      
      Oring.rt = index_finger.GetComponent<RectTransform>();
      Oring.sx = Oring.sy = (distance(hand.ring_finger_tip_X,
                                        hand.ring_finger_tip_Y,
                                        hand.ring_finger_mcp_X,
                                        hand.ring_finger_mcp_Y))/((float)(Oring.rt.rect.height));

      // pinky finger
      Opinky.x = (hand.pinky_mcp_X + hand.pinky_pip_X + hand.pinky_dip_X + hand.pinky_tip_X)/4;
      Opinky.y = (hand.pinky_mcp_Y + hand.pinky_pip_Y + hand.pinky_dip_Y + hand.pinky_tip_Y)/4;
      dy = (float)(hand.pinky_tip_Y-hand.pinky_mcp_Y);
      dx = (float)(hand.pinky_tip_X-hand.pinky_mcp_X);
      Opinky.rz = ((int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg)-90);
      
      Opinky.rt = pinky_finger.GetComponent<RectTransform>();
      Opinky.sx = Opinky.sy = (distance(hand.pinky_tip_Y,
                                        hand.pinky_tip_X,
                                        hand.pinky_mcp_Y,
                                        hand.pinky_mcp_X))/((float)(Opinky.rt.rect.height));
    }
    // Update is called once per frame
    void Update()
    {
        // get data values from stored in udpReceiver "data" field
        json_data = manager.GetComponent<udpReceiver>().data;
        // Debug.Log(json_data);
        // convert/decode json value received from python script into integer values
        hand = JsonUtility.FromJson<hand_landmark>(json_data);
        // hand.print(); //for debugging purpose
        // this function updates state variables associated with hand palm like centroid, rotation and scale
        update_palm();
        // this function updates state variables associated with hand fingers for fixing centre, slope, rotations and scaling
        update_fingers();

        // use state varaibles of hand palm to transform palm tattoo image
        palm.transform.position = new Vector3(palm_x*Screen.width/640,palm_y*Screen.height/480,palm.transform.position.z);
        palm.transform.rotation = Quaternion.Euler(0,0,palm_rz);
        palm.transform.localScale = new Vector3(palm_sx, palm_sy, palm_sz);


        // use state variables of hand fingers to transform hand fingers tatoo images
        finger_thumb.transform.position = new Vector3(Othumb.x*Screen.width/640,Othumb.y*Screen.height/480,finger_thumb.transform.position.z);
        finger_thumb.transform.rotation = Quaternion.Euler(Othumb.rx, Othumb.ry, Othumb.rz);
        finger_thumb.transform.localScale = new Vector3(Othumb.sx, Othumb.sy, Othumb.sz);

        index_finger.transform.position = new Vector3(Oindex.x*Screen.width/640,Oindex.y*Screen.height/480,index_finger.transform.position.z);
        index_finger.transform.rotation = Quaternion.Euler(Oindex.rx, Oindex.ry, Oindex.rz);
        index_finger.transform.localScale = new Vector3(Oindex.sx, Oindex.sy, Oindex.sz);
        
        middle_finger.transform.position = new Vector3(Omiddle.x*Screen.width/640,Omiddle.y*Screen.height/480,middle_finger.transform.position.z);
        middle_finger.transform.rotation = Quaternion.Euler(Omiddle.rx, Omiddle.ry, Omiddle.rz);
        middle_finger.transform.localScale = new Vector3(Omiddle.sx, Omiddle.sy, Omiddle.sz);
        
        ring_finger.transform.position = new Vector3(Oring.x*Screen.width/640,Oring.y*Screen.height/480,ring_finger.transform.position.z);
        ring_finger.transform.rotation = Quaternion.Euler(Oring.rx, Oring.ry, Oring.rz);
        ring_finger.transform.localScale = new Vector3(Oring.sx, Oring.sy, Oring.sz);
        
        pinky_finger.transform.position = new Vector3(Opinky.x*Screen.width/640,Opinky.y*Screen.height/480,pinky_finger.transform.position.z);
        pinky_finger.transform.rotation = Quaternion.Euler(Opinky.rx, Opinky.ry, Opinky.rz);
        pinky_finger.transform.localScale = new Vector3(Opinky.sx, Opinky.sy, Opinky.sz);
    }
}
