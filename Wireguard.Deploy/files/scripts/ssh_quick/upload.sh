set -e
CONF="$1"
DIR="$2"
FILE="$3"
IF="$4"
ENABLE="$5"
SERVICE="wg-quick@$IF"

if [ "$ENABLE" != "" ]; then
  systemctl enable $SERVICE
fi

if ! ls "$DIR" 2> /dev/null; then
  echo "$DIR not exist. Create"
  mkdir -p "$DIR"
fi

echo "Write to $DIR/$FILE"
echo "$CONF" | tee "$DIR/$FILE" > /dev/null

exit 0
