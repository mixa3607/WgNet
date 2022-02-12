set -e
DIR="$1"
FILE="$2"
IF="$2"
FULL="$DIR/$FILE"

SERVICE="wg-quick@$IF"
if !systemctl disable $SERVICE; then
  echo "Cant disable $SERVICE"
else
  echo "Service $SERVICE disabled"
fi

if test -f "$FULL"; then
  rm "$FULL"
  exit 0
else
  echo "$FULL not exist"
  exit 1
fi
