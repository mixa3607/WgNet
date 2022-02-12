set -e
which wg > /dev/null
if [ $? != 0 ]; then
  echo "wg not found"
  exit 1
fi

which uci > /dev/null
if [ $? != 0 ]; then
  echo "uci not found"
  exit 1
fi

wg -v | head -1

if [ `uci changes | wc -l` != 0 ]; then
  echo "Have uncommited changes"
  exit 1
fi

exit 0
