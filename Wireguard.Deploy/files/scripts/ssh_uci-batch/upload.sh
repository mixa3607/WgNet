set -e
BATCH="$1"
IF_NAME="$2"
ZONE="$3"
AUTOSTART="$4"

echo "Apply batch"
uci batch <<EOF
$BATCH
EOF

if [ "$ZONE" != "" ]; then
  zoneIdx=''
  i=0
  while uci get firewall.@zone[$i] > /dev/null 2>&1 ; do
  	if [ `uci get firewall.@zone[$i].name` == "$ZONE" ]; then
      zoneIdx=$i
    fi
  	i=$((i+1));
  done

  if [ $zoneIdx == '' ]; then
    echo "Zone not found"
    exit 1
  else
    echo "Add to zone"
    uci add_list firewall.@zone[$zoneIdx].network="$IF_NAME"
  fi
fi

if [ "$AUTOSTART" != "" ]; then
  echo "Add $IF_NAME to autostart"
  uci set network.$IF_NAME.auto=1
fi

echo Commit
uci commit

echo Reload network and firewall
/etc/init.d/network reload
/etc/init.d/firewall reload

exit 0
