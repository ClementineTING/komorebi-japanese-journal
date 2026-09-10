const fs=require('fs'),vm=require('vm'),assert=require('assert'),crypto=require('crypto');let saved;
const context=vm.createContext({state:{custom:[]},DATA:[],byId:new Map(),crypto,localStorage:{setItem(k,v){saved=v}},console});vm.runInContext(fs.readFileSync(require('path').join(__dirname,'custom-source.js'),'utf8'),context);
function run(s){return vm.runInContext(s,context)}
assert.equal(run('validateCSV("\\uFEFF单词,假名,释义\\r\\n桜,さくら,樱花").length'),1);
assert.equal(run('validateCSV(\'单词,假名,释义,例句\\n桜,さくら,"樱花,花","一行\\n二行"\')[0].ex'),'一行\n二行');
for(const s of ['单词,释义\n桜,花','单词,假名,释义\n桜,,花','单词,假名,释义\n桜,さくら,"花','单词,假名,释义\n桜,さくら,花,extra','单词,单词,释义\n桜,さくら,花'])assert.throws(()=>run('validateCSV('+JSON.stringify(s)+')'));
run('var words=validateCSV("单词,假名,释义\\n桜,さくら,樱花\\n桜,さくら,花")');assert.equal(run('importWords(words).added'),1);assert.equal(run('importWords(words).skipped'),2);assert.equal(JSON.parse(saved).custom.length,1);context.localStorage.setItem=()=>{throw Error('quota')};assert.throws(()=>run('importWords(validateCSV("单词,假名,释义\\n花,はな,花"))'));assert.equal(run('state.custom.length'),1);console.log('PASS: UTF-8 BOM, CRLF, quoted commas/newlines, malformed rows, duplicate keys, persistence, atomic quota failure');
