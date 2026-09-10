import json, pathlib, urllib.request, tarfile, io
root=pathlib.Path(__file__).resolve().parents[1]
old={w['id']:w for w in json.loads((root/'data/vocabulary.json').read_text())}
req=urllib.request.Request('https://api.github.com/repos/scriptin/jmdict-simplified/releases/latest',headers={'User-Agent':'Komorebi-updater'})
release=json.load(urllib.request.urlopen(req))
asset=next(a for a in release['assets'] if 'eng-common' in a['name'] and a['name'].endswith('.tgz'))
with tarfile.open(fileobj=io.BytesIO(urllib.request.urlopen(asset['browser_download_url']).read())) as archive:
    raw=json.load(archive.extractfile(next(m for m in archive.getmembers() if m.name.endswith('.json'))))
words=[]
for w in raw['words']:
    k=w['kanji'];r=w['kana'];senses=w['sense']
    row={'id':w['id'],'w':next((x['text'] for x in k if x['common']),k[0]['text'] if k else r[0]['text']),'r':r[0]['text'],'en':' / '.join('; '.join(g['text'] for g in s['gloss']) for s in senses),'pos':', '.join(dict.fromkeys(p for s in senses for p in s['partOfSpeech'])),'aliases':' '.join(x['text'] for x in k+r)}
    previous=old.get(w['id'],{})
    for key in ['zh','ex','cn']:
        if key in previous:row[key]=previous[key]
    if 'zh' in previous:row['r']=previous['r']
    words.append(row)
(root/'data/vocabulary.json').write_text(json.dumps(words,ensure_ascii=False,separators=(',',':')))
print('Updated',len(words),'words, date',raw['dictDate'])
print('Check vocabulary count/date in template and README, then run npm run build and npm test.')
