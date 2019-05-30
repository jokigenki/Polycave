# Prerequisites
# install pip (or add to PATH)
# pip install --upgrade pip google-api-python-client google-auth-httplib2 google-auth-oauthlib

from __future__ import print_function
import io
import os
import json
import pickle
import os.path
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request

import sys

reload(sys)
sys.setdefaultencoding('utf8')

SCOPES = ['https://www.googleapis.com/auth/spreadsheets.readonly']


class DataGenerator:

    def __init__(self):
        reload(sys)
        sys.setdefaultencoding('utf8')

    def SetupSheetsAPI(self):

        creds = None
        # The file token.pickle stores the user's access and refresh tokens, and is
        # created automatically when the authorization flow completes for the first
        # time.
        if os.path.exists('token.pickle'):
            with open('token.pickle', 'rb') as token:
                creds = pickle.load(token)
        # If there are no (valid) credentials available, let the user log in.
        if not creds or not creds.valid:
            if creds and creds.expired and creds.refresh_token:
                creds.refresh(Request())
            else:
                flow = InstalledAppFlow.from_client_secrets_file(
                    'credentials.json', SCOPES)
                creds = flow.run_local_server()
            # Save the credentials for the next run
            with open('token.pickle', 'wb') as token:
                pickle.dump(creds, token)

        self.service = build('sheets', 'v4', credentials=creds)

    def CallSheetsAPI(self, spreadsheet_id, range_name):
        result = self.service.spreadsheets().values().get(spreadsheetId=spreadsheet_id,
                                                          range=range_name).execute()
        self.values = result.get('values', [])

    def ChurnLocalisationFilesAndSpreadsheetIntoCharacterArray(self, characters):

        colMax = ord(WORDS_RANGE_NAME.split(":")[1])-65
        if not self.values:
            print('No data found.')
        else:
            i = 0
            print("Processing {} rows and {} columns".format(
                len(self.values), colMax))
            # Iterate every character in the spreadsheet and produce an array of characters
            for row in self.values:
                i = i + 1
                rowMax = min(len(row), colMax)
                # print('------------------------------')
                #print('row: {}'.format(i))
                for col in range(0, rowMax):
                    #print('col: {}'.format(chr(col + 65)))
                    value = row[col]
                    # print('{}'.format(value))
                    for c in value:
                        if not c in characters:
                            characters.append(c)
                            #print('val: {}'.format(c))
                # print('------------------------------')

        print("Complete with %d characters found" % (len(characters)))

    def CreateCharacterList(self, characters):
        self.ChurnLocalisationFilesAndSpreadsheetIntoCharacterArray(characters)
        return


characters = []

params = sys.argv
destination = "../Assets/" + params[1]
pCount = len(params)
print("destination {}".format(destination))
print("creating from {} file(s)".format(pCount - 2))
for i in range(2, pCount):
    vals = params[i].split("|")
    # Get character list from sheet
    SPREADSHEET_ID = vals[0]
    print("Parsing sheet {}".format(SPREADSHEET_ID))
    WORDS_RANGE_NAME = ""
    fromRange = ""
    for j in range(1, len(vals)):
        WORDS_RANGE_NAME = vals[j]
        print("Parsing range: {}".format(WORDS_RANGE_NAME))
        dg = DataGenerator()
        dg.SetupSheetsAPI()
        dg.CallSheetsAPI(SPREADSHEET_ID, WORDS_RANGE_NAME)
        dg.CreateCharacterList(characters)

# write the combined file
characters = sorted(characters)
charString = ''.join(characters)


try:
    os.remove(destination)
except OSError:
    pass
with io.open(destination, 'a+', encoding='utf-8') as f:
    f.write(unicode(charString))
