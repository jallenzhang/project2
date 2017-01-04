/***********************************************************************
        filename:   MaskEffect.cs
        created:    2012.04.28
        author:     Twj

        purpose:    ����Ч������
*************************************************************************/

using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;
using DataPersist;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class MaskEffect : MonoBehaviour
{
    // public variables

    // true�������ݽ��ڿ�ʼʱ���£�false��������ÿ֡�������
    public bool     isStatic = true;

    public Color    color = new Color( 0.0f, 0.0f, 0.0f, 0.5f );    // ���ֶ�����ɫ
    /////////////////////////////////////////////////////////////////////

    // protected variables

    protected bool          playing;    // �Ƿ����ڲ�������Ч��
    protected float         startTime;  // ����Ч����ʼʱ��
    protected float         totalTime;  // ����Ч����ʱ��
    protected MeshFilter    meshFilter;

    protected delegate void PreStartCallback();
    protected PreStartCallback  preStartCallback;   // ���ֿ�ʼǰ�Ļص�����

    protected delegate void EndCallback();
    protected EndCallback       endCallback;        // ���ֽ����Ļص�����
	
	private AudioSource AudioSource {get;set;}
	
	public GameObject role;
	
	private bool played=false;
 	
     /////////////////////////////////////////////////////////////////////

    // public functions

    // ��ʼ����Ч��
    // @params: time - ����Ч����ʱ��
    public virtual void StartEffect( float time )
    {
        playing     = true;
        startTime   = Time.time;
        totalTime   = time;

        //
        if ( null != preStartCallback )
        {
            preStartCallback();
        }

        //
        renderer.enabled = true;
    }
    /////////////////////////////////////////////////////////////////////

    // protected functions

    protected virtual void Awake()
    {
        InitMesh();

        //
        renderer.castShadows = false;
        renderer.receiveShadows = false;
		
		AudioSource=gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Update()
    {
         if ( !isStatic )
        {
            InitMeshColors( meshFilter.mesh );
        }
			

        //
        if ( playing )
        {
			 
            float deltaTime = Time.time - startTime;    // Ч���Ѳ��ŵ�ʱ��
			//Debug.Log("deltaTime:"+deltaTime+"totalTime:"+totalTime);
			if(totalTime-deltaTime<3.0f && played==false)
			{		//	Debug.Log("deltaTime:"+deltaTime+"totalTime:"+totalTime);
				played=true;
				 SoundHelper.PlaySound("Music/Other/Last3SencondThinkingTime",AudioSource,0);

			}
            if ( deltaTime > totalTime )
            {
                playing = false;
				
				Destroy(role);
				TableInfo infor= Room.Singleton.PokerGame.TableInfo;
				if(infor!=null)
				{
					if(User.Singleton.UserData.NoSeat!=-1)
					{
					   if(User.Singleton.UserData.NoSeat==infor.NoSeatCurrPlayer)
					   {
							PhotonClient.Singleton.AutoFold();
						  //TableState.Singleton.Fold();	
					   }
					}
				}
				 
				//Debug.LogError("KKKKK");
                 //
                if ( null != endCallback )
                {
                    endCallback();
					
					
                }

                //
                renderer.enabled = false;
				
				 
            }
            else
            {
                UpdateEffect();
            }
        }
    }

    // ��ʼ��Mesh
    // @return: true��ʼ��Mesh�ɹ���false��ʼ��Meshʧ��
    protected virtual bool InitMesh()
    {
        meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = new Mesh();
        InitMeshVertices( meshFilter.mesh );
        InitMeshIndices( meshFilter.mesh );
        InitMeshColors( meshFilter.mesh );

        return true;
    }

    // ��ʼ��Mesh����
    // @return: true��ʼ��Mesh����ɹ���false��ʼ��Mesh����ʧ��
    protected virtual bool InitMeshVertices( Mesh mesh )
    {
        return true;
    }

    // ��ʼ��Mesh����
    // @return: true��ʼ��Mesh�����ɹ���false��ʼ��Mesh����ʧ��
    protected virtual bool InitMeshIndices( Mesh mesh )
    {
        return true;
    }

    // ��ʼ��Mesh��ɫ
    // @return: true��ʼ��Mesh��ɫ�ɹ���false��ʼ��Mesh��ɫʧ��
    protected virtual bool InitMeshColors( Mesh mesh )
    {
        Color[] colors = new Color[mesh.vertices.Length];
        for ( int i = 0; i < colors.Length; ++i )
        {
            colors[i] = color;
        }

        mesh.colors = colors;

        return true;
    }

    // ��������Ч��
    protected virtual void UpdateEffect()
    {}
    /////////////////////////////////////////////////////////////////////
}