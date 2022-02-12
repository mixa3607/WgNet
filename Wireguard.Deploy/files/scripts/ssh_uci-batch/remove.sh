set -e
IF_NAME="$1"
ZONE="$2"

echo "Delete network"
uci delete "network.$IF_NAME"

echo "Delete peers"
while uci delete "network.@wireguard_$IF_NAME[-1]" &>/dev/null; do : ; done

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
    echo "Delete from zone"
    uci del_list firewall.@zone[$zoneIdx].network="$IF_NAME"
  fi
fi

echo Commit
uci commit

echo Reload network and firewall
/etc/init.d/network reload
/etc/init.d/firewall reload

exit 0
