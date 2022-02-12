set -e
which wg > /dev/null
if [ $? != 0 ]; then
  echo "wg not found"
  exit 1
fi

which wg-quick > /dev/null
if [ $? != 0 ]; then
  echo "wg-quick not found"
  exit 1
fi

wg -v | head -1
WG_OUT=`wg`
echo "$WG_OUT"| grep "^interface:" || true

exit 0
