import cv2
import mediapipe as mp
import time
import socket
import json

class handDetector():
    def __init__(self, mode = False, maxHands = 1, modelComplexity=1, detectionCon = 0.5, trackCon = 0.5):
        self.mode = mode
        self.maxHands = maxHands
        self.modelComplex = modelComplexity
        self.detectionCon = detectionCon
        self.trackCon = trackCon

        self.mpHands = mp.solutions.hands
        self.hands = self.mpHands.Hands(self.mode, self.maxHands, self.modelComplex, self.detectionCon, self.trackCon)
        self.mpDraw = mp.solutions.drawing_utils
        
    def findHands(self,img, draw = True):
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.hands.process(imgRGB)
        # print(self.results.multi_hand_landmarks,end='\n\n\n')

        if self.results.multi_hand_landmarks:
            for handLms in self.results.multi_hand_landmarks:
                if draw:
                    self.mpDraw.draw_landmarks(img, handLms, self.mpHands.HAND_CONNECTIONS)
        return img

    def findPosition(self, img, handNo = 0, draw = True):

        lmlist = []
        if self.results.multi_hand_landmarks:
            myHand = self.results.multi_hand_landmarks[handNo]
            for id, lm in enumerate(myHand.landmark):
                h, w, c = img.shape
                cx, cy = int(lm.x * w), int(lm.y * h)
                lmlist.append([id, cx, cy])
                if draw:
                    cv2.circle(img, (cx, cy), 3, (255, 0, 255), cv2.FILLED)
        return lmlist

def main():
    pTime = 0
    cTime = 0
    cap = cv2.VideoCapture(1) #!!!change to 1
    detector = handDetector()
    width = cap.get(cv2.CAP_PROP_FRAME_WIDTH )
    height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT )
    print(f'width={width}, height={height}')
    # create udp socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    serverAddressPort = ("127.0.0.1", 10050)
    info = ["wrist",
            "thumb_cmc",
            "thumb_mcp",
            "thumb_ip",
            "thumb_tip",
            "index_finger_mcp",
            "index_finger_pip",
            "index_finger_dip",
            "index_finger_tip",
            "middle_finger_mcp",
            "middle_finger_pip",
            "middle_finger_dip",
            "middle_finger_tip",
            "ring_finger_mcp",
            "ring_finger_pip",
            "ring_finger_dip",
            "ring_finger_tip",
            "pinky_mcp",
            "pinky_pip",
            "pinky_dip",
            "pinky_tip"            
            ]
    while True:
        success, img = cap.read()
        img = detector.findHands(img)
        lmlist = detector.findPosition(img)
        # if len(lmlist) != 0:
        #     print(lmlist[4])

        cTime = time.time()
        fps = 1 / (cTime - pTime)
        pTime = cTime

        cv2.putText(img, str(int(fps)), (10, 70), cv2.FONT_HERSHEY_PLAIN, 3, (255, 0, 255), 3)

        cv2.imshow("Image", img)
        data = {}
        data["width"] = int(width)
        data["height"] = int(height)
        if len(lmlist)!=21:
            lmlist = [(-1000, -10000, -10000)]*21
        for i,d in enumerate(lmlist):
            data[info[i]+"_X"] = d[1]
            data[info[i]+"_Y"] = height-d[2]
        json_data = json.dumps(data)
        # print(json_data)
        sock.sendto(json_data.encode('utf-8'), serverAddressPort)

        cv2.waitKey(1)


if __name__ == "__main__":
    main()