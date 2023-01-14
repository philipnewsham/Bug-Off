using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Web
{
    public WebPoint[] webPoints;
    public WebLine[] webLines;

    public Web(WebPoint[] points, WebLine[] lines)
    {
        webPoints = points;
        webLines = lines;
    }

    public WebPoint ReturnNextClosestWebPoint(WebPoint currentWebPoint, WebPoint targetWebPoint)
    {
        WebPoint[] m_connectedWebPoints = currentWebPoint.ReturnConnectedPoints();

        if(m_connectedWebPoints.Length == 0)
        {
            return currentWebPoint;
        }

        WebPoint m_closestPoint = m_connectedWebPoints[0];
        float m_closestDistance = Vector2.Distance(m_closestPoint.transform.position, targetWebPoint.transform.position);
        for (int i = 1; i < m_connectedWebPoints.Length; i++)
        {
            float m_distanceFromTarget = Vector2.Distance(m_connectedWebPoints[i].transform.position, targetWebPoint.transform.position);
            if(m_distanceFromTarget < m_closestDistance)
            {
                m_closestPoint = m_connectedWebPoints[i];
                m_closestDistance = m_distanceFromTarget;
            }
        }

        return m_closestPoint;
    }
}
